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
                                             IProductReadRepository productReadRepository,
                                             ITrackedProductReadRepository trackedProductReadRepository) : base(context)
        {
            _htmlWeb = new HtmlWeb();
            _productReadRepository = productReadRepository;
            _trackedProductReadRepository = trackedProductReadRepository;
        }

        public async Task<Response> AddProductTracking(AddProductTrackingDto addTrackingProductDto)
        {
            var product = await _productReadRepository.GetWhere(x => x.Id == addTrackingProductDto.ProductId,true).FirstOrDefaultAsync();
            var hasValue = await _trackedProductReadRepository.GetWhere(x => x.ProductId == addTrackingProductDto.ProductId).FirstOrDefaultAsync();

            if (product == null)
            {
                return new Response(ResponseCode.NotFound, "Product was not found for tracking");
            }

            if (hasValue != null) 
            {
                return new Response(ResponseCode.BadRequest, "This product already been tracking.");
            }
            
            HtmlDocument doc = _htmlWeb.Load(product.Url);
            var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen").InnerText.Replace("TL", String.Empty));

            if (price == null)
            {
                return new Response(ResponseCode.Error, "Product has not a price.");
            }

            using (var trackedProduct = new TrackedProduct())
            {
                trackedProduct.ProductId = product.Id;
                trackedProduct.Interval = addTrackingProductDto.Interval;
                trackedProduct.CurrentPrice = Math.Round(price,2);
                trackedProduct.TargetPrice = addTrackingProductDto.TargetPrice;
                trackedProduct.PriceHistory = new string[] { String.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString()) };
                trackedProduct.NextRunTime = DateTime.Now.AddMinutes(addTrackingProductDto.Interval);
                
                await AddAsync(trackedProduct);
                
                var result = await SaveChangesAsync();
                if (result == 0)
                {
                    
                    return new Response(ResponseCode.Error, "Error on add.");
                }
                product.isTracking = true;
                _productReadRepository.SaveChangesAsync();
                return new Response(ResponseCode.Success, "Tracked product added succesfully.");
            }

            return null;

        }


        public async Task<Response> DeleteTrackingProduct(int productId)
        {
            var trackingProduct = await _trackedProductReadRepository.GetSingleAsync(x => x.ProductId == productId);

            if (trackingProduct == null)
            {
                return new Response(ResponseCode.NotFound, "Tracking product not found");
            }

            bool state = Remove(trackingProduct);
            if (state == true)
            {
                return new Response(ResponseCode.Success, "Product deleted from tracking list succesfully");
            }
            else
            {
                return new Response(ResponseCode.Error, "Error on deleting");
            }


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
