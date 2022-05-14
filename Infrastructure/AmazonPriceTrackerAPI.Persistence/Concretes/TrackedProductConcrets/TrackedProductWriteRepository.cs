using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes.TrackedProductConcrets
{
    public class TrackedProductWriteRepository : WriteRepository<TrackedProduct>, ITrackedProductWriteRepository 
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly ITrackedProductReadRepository _trackedProductReadRepository;

        private readonly HtmlWeb _htmlWeb;

        public TrackedProductWriteRepository(AmazonPriceTrackerDbContext context,                      
                                             IProductReadRepository productReadRepository) : base(context)
        {
            _htmlWeb = new HtmlWeb();
            _htmlWeb.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36";
            _productReadRepository = productReadRepository;
        }


        public async Task<Response> AddProductTracking(AddProductTrackingDto addTrackingProductDto)
        {
            var product = _productReadRepository.GetWhere(x => x.Id == addTrackingProductDto.ProductId).FirstOrDefault();
            
            if (product == null)
            {
                return new Response(ResponseCode.NotFound, "Product was not found for tracking");
            }
            
            HtmlDocument doc = _htmlWeb.Load(product.Url);
            var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span").InnerText.Replace("TL", String.Empty));
            if (price == null)
            {
                return new Response(ResponseCode.Error, "Product has not a price.");
            }

            using (var trackedProduct = new TrackedProduct())
            {

                trackedProduct.ProductId = product.Id;
                trackedProduct.Interval = addTrackingProductDto.Interval;
                trackedProduct.CurrentPrice = price;
                trackedProduct.TargetPrice = addTrackingProductDto.TargetPrice;
                trackedProduct.PriceHistory = new string[] { String.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString()) };
                trackedProduct.NextRunTime = DateTime.Now.AddMinutes(addTrackingProductDto.Interval);

                await AddAsync(trackedProduct);
                var result = await SaveChangesAsync();
                if (result == 0)
                {
                    return new Response(ResponseCode.Error, "Error on add.");
                }
                return new Response(ResponseCode.Success, "Tracked product added succesfully.");
            }

            return null;

        }

        
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
            if(checkTrackingProductDtos == null) 
            {
                return null;
            }

            List<string> result = new List<string>();

            foreach (var item in checkTrackingProductDtos)
            {
                HtmlDocument doc = _htmlWeb.Load(item.Url);
                var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span").InnerText.Replace("TL", String.Empty));
                var trackedProduct = await _trackedProductReadRepository.GetByIdAsync(item.TrackingId, true);
                var product = await _productReadRepository.GetByIdAsync(item.ProductId,true);
                
                if (trackedProduct.TargetPrice > price) 
                {
                    trackedProduct.CurrentPrice = price;
                    trackedProduct.PriceHistory.Append(String.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString()));
                    trackedProduct.NextRunTime = DateTime.Now.AddMinutes(trackedProduct.Interval);
                    product.CurrentPrice = price;
                    
                    result.Append(string.Format("Tracked for tracking id: {0}",item.TrackingId));
                    //_trackedProductWriteRepository.Update(trackedProduct);
                    await SaveChangesAsync();
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
        }

    }
}
