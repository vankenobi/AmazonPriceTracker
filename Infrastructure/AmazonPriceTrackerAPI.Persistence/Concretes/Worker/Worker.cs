using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Application.Repositories.MailRepo;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using AmazonPriceTrackerAPI.Infrastructure.Concrets;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Mail;

namespace AmazonPriceTrackerAPI.Persistence.Concretes.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;
        private readonly HtmlWeb _htmlWeb;
        private readonly IMailRepository _mailRepository;
        private readonly ITrackedProductReadRepository _trackedProductReadRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly AmazonPriceTrackerDbContext _context;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _provider = serviceProvider;
            _htmlWeb = new HtmlWeb();
            _htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.54 Safari/537.36";

            IServiceScope scope = _provider.CreateScope();

            _context = scope.ServiceProvider.GetRequiredService<AmazonPriceTrackerDbContext>();
            _mailRepository = scope.ServiceProvider.GetRequiredService<IMailRepository>();
            _trackedProductReadRepository = scope.ServiceProvider.GetRequiredService<ITrackedProductReadRepository>();
            _productReadRepository = scope.ServiceProvider.GetRequiredService<IProductReadRepository>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await CheckTrackingProductNextRunTime();
                //await SendEmail(new MailTemplateDto { CurrentPrice=1905.55551 , OldPrice = 1800.0000, Discount = 105.00 ,ImagePath = "https://m.media-amazon.com/images/I/71yofUBMxSL._AC_SX679_.jpg",Title = "Apple Watch SE GPS, 40 mm Gümüş Rengi Alüminyum Kasa ve Koyu Abis Spor Kordon - Normal Boy" });
                await deneme(result);
                await Task.Delay(10000);
            }
        }

        public async Task<List<CheckTrackingProductDto>> CheckTrackingProductNextRunTime() 
        {
            var products = await _productReadRepository.GetAllAsync();
            var trackedProducts = await _trackedProductReadRepository.GetAllAsync();

            var result = (from p in products
                          join t in trackedProducts on p.Id equals t.ProductId
                          where t.NextRunTime < DateTime.Now
                          select new CheckTrackingProductDto { TrackingId = t.Id, ProductId = p.Id, Url = p.Url }).ToList();
            return result;
        }

        public async Task deneme(List<CheckTrackingProductDto> result) 
        {
            if (result != null)
            {
                List<string> runned = new List<string>();

                foreach (var item in result)
                {
                    HtmlDocument doc = await _htmlWeb.LoadFromWebAsync(item.Url);
                    // Read the price from webpage of product
                    var price = EditPrice(doc.DocumentNode.SelectSingleNode("//*[@id='corePrice_feature_div']/div/span/span[1]").InnerText.Replace("TL", string.Empty));

                    // Get TrackedProduct and product by ids
                    var trackedProduct = await _trackedProductReadRepository.GetSingleAsync(x => x.Id == item.TrackingId, true);
                    var product = await _productReadRepository.GetSingleAsync(x => x.Id == item.ProductId, true);

                    if (trackedProduct.CurrentPrice != price)
                    {
                        if (trackedProduct.TargetPrice > price)
                        {
                            trackedProduct.CurrentPrice = price;
                            product.CurrentPrice = price;
                            trackedProduct.MailSendingDate = DateTime.Now.Date;

                            trackedProduct.PriceChange = price - trackedProduct.CurrentPrice;

                            trackedProduct.PriceHistory = trackedProduct.PriceHistory.Append(string.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())).ToArray();
                            trackedProduct.NextRunTime = DateTime.Now.AddMinutes(trackedProduct.Interval);

                            runned.Append(string.Format("Tracked for tracking id: {0}", item.TrackingId));

                            _logger.LogInformation("Ürünün fiyatı düştü.");

                            await _context.SaveChangesAsync();
                        }
                        else if (trackedProduct.CurrentPrice < price)
                        {
                            trackedProduct.CurrentPrice = price;
                            product.CurrentPrice = price;

                            trackedProduct.PriceChange = price - trackedProduct.CurrentPrice;

                            trackedProduct.PriceHistory = trackedProduct.PriceHistory.Append(string.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())).ToArray();
                            trackedProduct.NextRunTime = DateTime.Now.AddMinutes(trackedProduct.Interval);

                            _logger.LogInformation("Ürünün fiyatı yükseldi.");

                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            _logger.LogInformation("logger runned succesfully.");
        }

        public async Task SendEmail(MailTemplateDto mailTemplateDto) 
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("pricetrackerappforamazon@gmail.com")
            };

            var emailList = await _context.Set<Email>().AsNoTracking().ToListAsync();

            foreach (var email in emailList) 
            {
                var template = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "../../../Infrastructure/AmazonPriceTrackerAPI.Infrastructure/HtmlTemplates/ProductPriceDown.html");
                var priceTrackerLogo = "https://drive.google.com/uc?export=view&id=1hzF_9nXM8s4oOLTh93u7u2Qy8motEBmd";
                
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(template);
                htmlDocument.GetElementbyId("product-img").SetAttributeValue("src", mailTemplateDto.ImagePath);
                htmlDocument.GetElementbyId("price-tracker-logo").SetAttributeValue("src", priceTrackerLogo);

                htmlDocument.GetElementbyId("old-price").InnerHtml = Math.Round(mailTemplateDto.OldPrice, 2).ToString();
                htmlDocument.GetElementbyId("new-price").InnerHtml = Math.Round(mailTemplateDto.CurrentPrice, 2).ToString();
                htmlDocument.GetElementbyId("discount").InnerHtml = Math.Round(mailTemplateDto.Discount, 2).ToString();
                htmlDocument.GetElementbyId("product-title").InnerHtml = mailTemplateDto.Title;
                
                mailMessage.Subject = "Amazon Price Tracker App";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = htmlDocument.DocumentNode.OuterHtml;
                mailMessage.To.Add(email.MailAddress);
                await _mailRepository.SendAnEmailAsync(mailMessage);
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
