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
        int counter = 0;
        int counter2 = 0;
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _provider = serviceProvider;
            _htmlWeb = new HtmlWeb();
            //_htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";

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
                counter++;
                _logger.LogInformation("Step : " + counter.ToString());
                await RunTasks(result);
                var rand = new Random();
                await Task.Delay(rand.Next(5 * 60000, 7 * 60000));
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

        // will catch error expections 
        public async Task RunTasks(List<CheckTrackingProductDto> result)
        {
            counter2 += result.Count;
            if (result != null)
            {
                foreach (var item in result)
                {
                    HtmlDocument doc = await _htmlWeb.LoadFromWebAsync(item.Url);

                    // Read the price from webpage of product
                    var random = new Random();
                    //await Task.Delay(random.Next(1000, 15000));
                    var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen").InnerText.Replace("TL", string.Empty));

                    // Get TrackedProduct and product by ids
                    TrackedProduct trackedProduct = await _trackedProductReadRepository.GetSingleAsync(x => x.Id == item.TrackingId, true);
                    Product product = await _productReadRepository.GetSingleAsync(x => x.Id == item.ProductId, true);
                    trackedProduct.NextRunTime = DateTime.Now.AddMinutes(trackedProduct.Interval);
                    await _trackedProductReadRepository.SaveChangesAsync();

                    _logger.LogInformation(counter2 + " kez çalıştı.");
                    if (trackedProduct.CurrentPrice != price)
                    {
                        if (trackedProduct.TargetPrice > price)
                        {
                            double oldPrice = (double)product.CurrentPrice;
                            trackedProduct.PriceChange = price - oldPrice;
                            trackedProduct.CurrentPrice = price;
                            product.CurrentPrice = price;
                            trackedProduct.MailSendingDate = DateTime.Now.Date;
                            trackedProduct.PriceHistory = trackedProduct.PriceHistory.Append(string.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())).ToArray();

                            MailTemplateDto mailTemplateDto = new()
                            {
                                CurrentPrice = trackedProduct.CurrentPrice,
                                Discount = trackedProduct.PriceChange,
                                ImagePath = product.Image,
                                OldPrice = (double)oldPrice,
                                Title = product.Name
                            };

                            //await SendEmailPriceDown(mailTemplateDto);
                            mailTemplateDto.Dispose();

                            _logger.LogInformation("Ürünün fiyatı düştü.");
                        }

                        else if (trackedProduct.CurrentPrice < price)
                        {
                            double oldPrice = (double)product.CurrentPrice;
                            trackedProduct.PriceChange = oldPrice - price;
                            trackedProduct.CurrentPrice = price;
                            product.CurrentPrice = price;
                            trackedProduct.MailSendingDate = DateTime.Now.Date;
                            trackedProduct.PriceHistory = trackedProduct.PriceHistory.Append(string.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())).ToArray();

                            MailTemplateDto mailTemplateDto = new()
                            {
                                CurrentPrice = trackedProduct.CurrentPrice,
                                Discount = trackedProduct.PriceChange,

                                ImagePath = product.Image,
                                OldPrice = (double)oldPrice,
                                Title = product.Name
                            };

                            // await SendEmailPriceUp(mailTemplateDto);
                            await _context.SaveChangesAsync();

                            mailTemplateDto.Dispose();
                            _logger.LogInformation("Ürünün fiyatı yükseldi.");
                        }

                        else if (trackedProduct.CurrentPrice > price)
                        {
                            double oldPrice = (double)product.CurrentPrice;
                            trackedProduct.PriceChange = oldPrice - price;
                            trackedProduct.CurrentPrice = price;
                            product.CurrentPrice = price;
                            trackedProduct.MailSendingDate = DateTime.Now.Date;
                            trackedProduct.PriceHistory = trackedProduct.PriceHistory.Append(string.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())).ToArray();
                        }

                        await _trackedProductReadRepository.SaveChangesAsync();
                        await _productReadRepository.SaveChangesAsync();
                    }
                }
            }
            _logger.LogInformation("logger runned succesfully.");
        }

        public async Task SendEmailPriceUp(MailTemplateDto mailTemplateDto)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("pricetrackerappforamazon@gmail.com")
            };

            var emailList = await _context.Set<Email>().Select(x => x.MailAddress).AsNoTracking().ToListAsync();
            var template = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "../../../Infrastructure/AmazonPriceTrackerAPI.Infrastructure/HtmlTemplates/ProductPriceUp.html");

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(template);

            htmlDocument.GetElementbyId("product-img").SetAttributeValue("src", mailTemplateDto.ImagePath);
            htmlDocument.GetElementbyId("old-price").InnerHtml = Math.Round(mailTemplateDto.OldPrice, 2).ToString("N2");
            htmlDocument.GetElementbyId("new-price").InnerHtml = Math.Round(mailTemplateDto.CurrentPrice, 2).ToString("N2");
            htmlDocument.GetElementbyId("discount").InnerHtml = Math.Round(mailTemplateDto.Discount, 2).ToString("N2");
            htmlDocument.GetElementbyId("product-title").InnerHtml = mailTemplateDto.Title;

            mailMessage.Subject = "The Price Of The Product Has Risen";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlDocument.DocumentNode.OuterHtml;

            foreach (var item in emailList)
            {
                mailMessage.To.Add(item);
            }

            await _mailRepository.SendAnEmailAsync(mailMessage);
        }

        public async Task SendEmailPriceDown(MailTemplateDto mailTemplateDto)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("pricetrackerappforamazon@gmail.com")
            };

            var emailList = await _context.Set<Email>().Select(x => x.MailAddress).AsNoTracking().ToListAsync();

            var template = await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + "../../../Infrastructure/AmazonPriceTrackerAPI.Infrastructure/HtmlTemplates/ProductPriceDown.html");

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(template);
            htmlDocument.GetElementbyId("product-img").SetAttributeValue("src", mailTemplateDto.ImagePath);

            htmlDocument.GetElementbyId("old-price").InnerHtml = Math.Round(mailTemplateDto.OldPrice, 2).ToString("N2");
            htmlDocument.GetElementbyId("new-price").InnerHtml = Math.Round(mailTemplateDto.CurrentPrice, 2).ToString("N2");
            htmlDocument.GetElementbyId("discount").InnerHtml = Math.Round(mailTemplateDto.Discount, 2).ToString("N2");
            htmlDocument.GetElementbyId("product-title").InnerHtml = mailTemplateDto.Title;

            mailMessage.Subject = "The Price Of The Product Has Dropped";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = htmlDocument.DocumentNode.OuterHtml;

            foreach (var item in emailList)
            {
                mailMessage.To.Add(item);
            }

            await _mailRepository.SendAnEmailAsync(mailMessage);

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
