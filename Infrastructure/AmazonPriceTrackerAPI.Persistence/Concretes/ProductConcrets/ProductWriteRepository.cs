using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes
{
    public class ProductWriteRepository : WriteRepository<Product>, IProductWriteRepository 
    {
        private readonly HtmlWeb _htmlWeb;
        private readonly ILogger _logger;
        private readonly IProductReadRepository _productReadRepository;
        private readonly ITrackedProductWriteRepository _trackedProductWriteRepository;
        private readonly ITrackedProductReadRepository _trackedProductReadRepository;

        public ProductWriteRepository(AmazonPriceTrackerDbContext context, 
                                      IProductReadRepository productReadRepository,
                                      ITrackedProductWriteRepository trackedProductWriteRepository,
                                      ITrackedProductReadRepository trackedProductReadRepository,
                                      ILogger logger) : base(context)
        {
            _htmlWeb = new HtmlWeb();
            _productReadRepository = productReadRepository;
            _trackedProductWriteRepository = trackedProductWriteRepository;
            _trackedProductReadRepository = trackedProductReadRepository;
            _logger = logger;
        }
        private async Task<bool> IsProductAlreadyExist(string url)
        {
            var hasValue = await _productReadRepository.GetWhere(x => x.Url == url).AnyAsync();
            if (hasValue)
                return true;
            return false;
        }

        public async Task<Response> AddNewProductWithUrlAsync(string url)
        {
            try
            {
                if(await IsProductAlreadyExist(url))
                {
                    _logger.Warning("Product already exists.");
                    return new Response(ResponseCode.BadRequest, "Product already exists.");
                }
                
                var doc = LoadDocument(url);
                var product = ParseProductFromHtml(doc,url);

                await AddAsync(product);
                await SaveChangesAsync();

                _logger.Information("Product added succesfully. {@product}",product);
                return new Response(ResponseCode.Success, "Product added succesfully.");
            }
            catch (Exception)
            {
                _logger.Information("Error on adding new product.");
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
                    _logger.Warning("The product to be deleted was not found");
                    return new Response(ResponseCode.NotFound, "Product not found.");
                }

                var trackingProduct = await _trackedProductReadRepository.GetSingleAsync(x => x.ProductId == product.Id,true);
                if (trackingProduct != null)
                {
                    await _trackedProductWriteRepository.Remove(trackingProduct.Id);
                    await _trackedProductWriteRepository.SaveChangesAsync(); 
                }

                await Remove(id); 
                await SaveChangesAsync();  
                _logger.Information("Product deleted succesfuly. {@product}", product);
                return new Response(ResponseCode.Success, "Product deleted succesfuly.");

            }
            catch (Exception ex)
            {
                _logger.Error("Error on delete: {ErrorMessage}", ex.Message);
                return new Response(ResponseCode.Error, "Error on delete");             
            }
        }

        public async Task<Response> ChangeFavoriteStateAsync(int productId) 
        {
            try
            {
                var product = await _productReadRepository.GetSingleAsync(x => x.Id == productId, true);

                if (product == null)
                {
                    _logger.Warning("The product not found");
                    return new Response(ResponseCode.NotFound, "The product not found");
                }

                product.isFavorite = !product.isFavorite;
                await _productReadRepository.SaveChangesAsync();
             
                _logger.Information("The product favorite state succesfully changed");
                return new Response(ResponseCode.Success, "The product favorite state succesfully changed");
            }
            catch (Exception ex)
            {
                _logger.Error("Error on update favorite state : {ErrorMessage}", ex.Message);
                return new Response(ResponseCode.Error, "Error on update favorite state.");
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

        private Product ParseProductFromHtml(HtmlDocument doc, string url)
        {
            var price = doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen");

            // Ürün fiyatı kontrolü
            if (price.InnerText == null)
            {
                _logger.Warning("Price of product not found - ", url);
            }

            Product product = new()
            {
                CurrentPrice = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen")),
                Name = doc.QuerySelector("#productTitle").InnerText.Trim(),
                Image = doc.QuerySelector("#landingImage").Attributes["src"].Value,
                StockState = doc.QuerySelector("#availability > span").InnerText.Trim(),
                Url = url,
                Rate = EditRate(doc.DocumentNode.SelectSingleNode("//*[@id='acrPopover']/span[1]/a/i[1]/span")?
                                .InnerText.Split(" ")[3]),
                TechnicalDetails = doc.DocumentNode.SelectNodes(@"//*[@id='feature-bullets']/ul//li")?
                                        .Select(li => li.InnerText).ToList<string>(),
                Description = doc.QuerySelector("#productDescription > p > span")?.InnerText
            };

            return product;
        }

        private HtmlDocument LoadDocument(string url){
            try
            {
                HtmlDocument doc = _htmlWeb.Load(url);
                _logger.Information("Product page parsed.");
                return doc;
            }
            catch (System.Exception ex)
            {
                _logger.Error("Error on product page pull",ex);
            }
            return null;
        }


    }
}
