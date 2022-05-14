
using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Amazon_Price_Tracker.Worker
{
    public class Worker : BackgroundService, IHostedService
    {
        
        private readonly ILogger<Worker> _logger;
        private readonly ITrackedProductWriteRepository _trackedProductWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly ITrackedProductReadRepository _trackedProductReadRepository;
        private readonly HtmlWeb _htmlWeb;
        
        public Worker(ILogger<Worker> logger, ITrackedProductWriteRepository trackedProductWriteRepository,
                                              IProductReadRepository productReadRepository)
        {
            _logger = logger;
            //_trackedProductWriteRepository = trackedProductWriteRepository;
            _productReadRepository = productReadRepository;
            _htmlWeb = new HtmlWeb();
            _htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36";
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {/*
                List<CheckTrackingProductDto> checkTrackingProductDto = await _trackedProductWriteRepository.ListProductsThatHaveBeenTracked();
                if (checkTrackingProductDto != null)
                {
                    List<string> checkedProducts = await _trackedProductWriteRepository.CheckPriceOfProductThatHasBeenTracked(checkTrackingProductDto);
                    _logger.LogInformation(checkedProducts.ToString());
                    _logger.LogInformation("Log Çalıştı. " + DateTime.Now.ToString());
                }*/
                await Task.Delay(10000);
            }
        }
        /*
        public async Task<List<CheckTrackingProductDto>> ListProductsThatHaveBeenTracked()
        {
            var products = await _productReadRepository.GetAll().ToListAsync();
            var trackedProducts = await _trackedProductReadRepository.GetAll().ToListAsync();

            var result = (from p in products
                          join t in trackedProducts on p.Id equals t.ProductId
                          where t.NextRunTime < DateTime.Now
                          select new CheckTrackingProductDto { TrackingId = t.Id, ProductId = p.Id, Url = p.Url }).ToList();
            return result;
        }

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
        }*/
    }
}
