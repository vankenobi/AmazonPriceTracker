using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
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
        private readonly ITrackedProductWriteRepository _trackedProductWriteRepository;
        private readonly ITrackedProductReadRepository _trackedProductReadRepository;
        private readonly ILogger<ProductWriteRepository> _logger;

        public ProductWriteRepository(AmazonPriceTrackerDbContext context, 
                                      IProductReadRepository productReadRepository,
                                      ITrackedProductWriteRepository trackedProductWriteRepository,
                                      ITrackedProductReadRepository trackedProductReadRepository,
                                      ILogger<ProductWriteRepository> logger) : base(context)
        {
            _htmlWeb = new HtmlWeb();
            _productReadRepository = productReadRepository;
            _trackedProductWriteRepository = trackedProductWriteRepository;
            _trackedProductReadRepository = trackedProductReadRepository;
            _logger = logger;
        }

        public async Task<Response> AddNewProductWithUrlAsync(string url)
        {
            try
            {
                HtmlDocument doc = _htmlWeb.Load(url);

                var hasValue = _productReadRepository.GetWhere(x => x.Url == url).FirstOrDefault();
                if (hasValue != null)
                {
                    return new Response(ResponseCode.BadRequest, "Product already exists.");
                }

                using (var product = new Product())
                {
                    var price = doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen");
                    if (price.InnerText == null)
                    {
                        _logger.LogInformation("Price of product not found");
                        return new Response(ResponseCode.Error, "Error on adding new product.");
                    }
                        
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

                    _logger.LogInformation("Product added succesfully.");
                    return new Response(ResponseCode.Success, "Product added succesfully.");
                }
            }
            catch (Exception)
            {
                _logger.LogInformation("Error on adding new product.");
                return new Response(ResponseCode.Error,"Error on adding new product.");
            }
        }


        public async Task<Response> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _productReadRepository.GetSingleAsync(x => x.Id == id);
                if (product == null)
                {
                    return new Response(ResponseCode.NotFound, "Product not found.");
                }

                var trackingProduct = await _trackedProductReadRepository.GetSingleAsync(x => x.ProductId == product.Id,true);
                if (trackingProduct != null)
                {
                    _trackedProductWriteRepository.Remove(trackingProduct.Id);
                }
                var state2 = Remove(id).Result;
                
                if (state2 == true)
                {
                    await SaveChangesAsync();
                    await _trackedProductWriteRepository.SaveChangesAsync();
                    return new Response(ResponseCode.Success, "Product deleted succesfuly.");
                }
                else
                {
                    return new Response(ResponseCode.Error, "Error on delete");
                }
            }
            catch (Exception)
            {
                return new Response(ResponseCode.Error, "Error on delete");
            }
        }

        public async Task<Response> ChangeFavoriteStateAsync(int productId) 
        {
            try
            {
                var productEntity = await _productReadRepository.GetSingleAsync(x => x.Id == productId, true);
                if (productEntity == null)
                {
                    return new Response(ResponseCode.NotFound, "Product not found");
                }

                productEntity.isFavorite = productEntity.isFavorite == false ? true : false;
                
                var state = Update(productEntity);
                await SaveChangesAsync();

                if (state == true)
                {
                    return new Response(ResponseCode.Success, "The product updated succesfully");
                }
                return new Response(ResponseCode.Error, "Error on update.");
            }
            catch (Exception)
            {
                return new Response(ResponseCode.Error, "Error on update.");
            }
            
           
        }


        private double? EditPrice(HtmlNode value)
        {
            if (value.InnerText.Contains(",") && value.InnerText.Contains("."))
            {
                return Math.Round(double.Parse(value.InnerText.Replace(".", string.Empty).Replace(",", ".").Replace("TL", String.Empty), CultureInfo.InvariantCulture.NumberFormat),2);
            }
            else if (value.InnerText.Contains(","))
            {
                return Math.Round(double.Parse(value.InnerText.Replace(",", ".").Replace("TL", String.Empty), CultureInfo.InvariantCulture.NumberFormat),2);
            }
            return null;
        }

        private double? EditRate(string value) 
        {
            if (value == null) 
            {
                return null;
            }
            if (value.Contains(","))
            {
                return double.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture.NumberFormat);
            }
            return null;
        }
    }
}
