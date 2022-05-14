
using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Amazon_Price_Tracker.Worker
{
    public class Worker : BackgroundService
    {
        
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;
        private readonly HtmlWeb _htmlWeb;
       
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _provider = serviceProvider;
            _htmlWeb = new HtmlWeb();
            _htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36";
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AmazonPriceTrackerDbContext>();

                    var products = await context.Set<Product>().ToListAsync();
                    var trackedProducts = await context.Set<TrackedProduct>().ToListAsync();

                    var result = (from p in products
                                  join t in trackedProducts on p.Id equals t.ProductId
                                  where t.NextRunTime < DateTime.Now //&& t.MailSendingDate.Date != DateTime.Now.Date
                                  select new CheckTrackingProductDto { TrackingId = t.Id, ProductId = p.Id, Url = p.Url }).ToList();

                    if (result != null) 
                    {
                        List<string> runned = new List<string>();

                        foreach (var item in result)
                        {
                            HtmlDocument doc = _htmlWeb.Load(item.Url);
                            
                            // Read the price from webpage of product
                            var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span").InnerText.Replace("TL", String.Empty));

                            // Get TrackedProduct and product by ids
                            var trackedProduct = await context.Set<TrackedProduct>().Where(x=>x.Id == item.TrackingId).FirstOrDefaultAsync();
                            var product = await context.Set<Product>().Where(x=>x.Id == item.ProductId).FirstOrDefaultAsync();

                            if (trackedProduct.CurrentPrice != price) 
                            {
                                if (trackedProduct.TargetPrice > price)
                                {
                                    trackedProduct.CurrentPrice = price;
                                    product.CurrentPrice = price;
                                    trackedProduct.MailSendingDate = DateTime.Now.Date;

                                    trackedProduct.PriceHistory = trackedProduct.PriceHistory.Append(String.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())).ToArray();
                                    trackedProduct.NextRunTime = DateTime.Now.AddMinutes(trackedProduct.Interval);

                                    runned.Append(string.Format("Tracked for tracking id: {0}", item.TrackingId));

                                    context.Set<TrackedProduct>().Update(trackedProduct);
                                    context.Set<Product>().Update(product);

                                    await context.SaveChangesAsync();
                                }
                            }
                            
                        }
                    }
                    _logger.LogInformation("logger runned succesfully.");
                    await Task.Delay(10000);
                }
            }
        }
        /*

        public async Task<List<string>> CheckPriceOfProductThatHasBeenTracked(List<CheckTrackingProductDto> checkTrackingProductDtos)
        {
            if (checkTrackingProductDtos == null)
            {
                return null;
            }

            List<string> result = new List<string>();

            foreach (var item in checkTrackingProductDtos)
            {
                HtmlDocument doc = _htmlWeb.Load(item.Url);
                var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span").InnerText.Replace("TL", String.Empty));
                var trackedProduct = await _trackedProductReadRepository.GetByIdAsync(item.TrackingId, true);
                var product = await _productReadRepository.GetByIdAsync(item.ProductId, true);

                if (trackedProduct.TargetPrice > price)
                {
                    trackedProduct.CurrentPrice = price;
                    trackedProduct.PriceHistory.Append(String.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString()));
                    trackedProduct.NextRunTime = DateTime.Now.AddMinutes(trackedProduct.Interval);
                    product.CurrentPrice = price;

                    result.Append(string.Format("Tracked for tracking id: {0}", item.TrackingId));
                    //_trackedProductWriteRepository.Update(trackedProduct);
                    await _trackedProductWriteRepository.SaveChangesAsync();
                }
            }
            return result;
        }

        */
        private double EditPrice(string value)
        {
            if (value.Contains(",") && value.Contains("."))
            {
                return double.Parse(value.Replace(".", string.Empty).Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (value.Contains(","))
            {
                return double.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
            }
            return double.Parse(value);
        }
    }
}
