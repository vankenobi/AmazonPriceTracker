using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes
{
    public class ProductWriteRepository : WriteRepository<Product>, IProductWriteRepository
    {
        private readonly HtmlWeb _htmlWeb;
        private readonly IProductReadRepository _productReadRepository;

        public ProductWriteRepository(AmazonPriceTrackerDbContext context, 
                                      IProductReadRepository productReadRepository) : base(context)
        {
            _htmlWeb = new HtmlWeb();
            _htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.67 Safari/537.36";
            _productReadRepository = productReadRepository;
        }

        public async Task<Response> AddNewProductWithUrl(string url)
        {
            HtmlDocument doc = _htmlWeb.Load(url);
            
            var hasValue = _productReadRepository.GetWhere(x=>x.Url == url).FirstOrDefault();
            if (hasValue != null)
            {
                return new Response(ResponseCode.Error,"Product already exists.");
            }

            using(var product = new Product())
            {
                product.CurrentPrice = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen"));
                product.Name = doc.QuerySelector("#productTitle").InnerText.Trim();
                product.Image = doc.QuerySelector("#landingImage").Attributes["src"].Value;
                product.StockState = doc.QuerySelector("#availability > span").InnerText.Trim();
                product.Url = url;      
                product.Rate = EditRate(doc.DocumentNode.SelectSingleNode("//*[@id='acrPopover']/span[1]/a/i[1]/span")?.InnerText.Split(" ")[3]);
                product.TechnicalDetails = doc.DocumentNode.SelectNodes(@"//*[@id='feature-bullets']/ul//li")?.Select(li => li.InnerText).ToList<string>();
                product.Description = doc.QuerySelector("#productDescription > p > span")?.InnerText;

                await AddAsync(product);
                await SaveChangesAsync();
           
                return new Response(ResponseCode.Success,"Product added succesfully.");
            }
            
            return null;
        }



        private double? EditPrice(HtmlNode value)
        {
            if (value.InnerText.Contains(",") && value.InnerText.Contains("."))
            {
                return float.Parse(value.InnerText.Replace(".", string.Empty).Replace(",", ".").Replace("TL", String.Empty), CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (value.InnerText.Contains(","))
            {
                return float.Parse(value.InnerText.Replace(",", ".").Replace("TL", String.Empty), CultureInfo.InvariantCulture.NumberFormat);
            }
            return null;
        }

        private double? EditRate(string value) 
        {
            if (value.Contains(","))
            {
                return float.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
            }
            return null;
        }
    }
}
