using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Persistence.Contexts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


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
            var isTracking = await _trackedProductReadRepository.GetWhere(x => x.ProductId == addTrackingProductDto.ProductId).AnyAsync();

            if (product == null)
            {
                return new Response(ResponseCode.NotFound, "Product was not found for tracking");
            }

            if (isTracking) 
            {
                return new Response(ResponseCode.BadRequest, "This product already been tracking.");
            }
            
            HtmlDocument doc = _htmlWeb.Load(product.Url);
            var price = EditPrice(doc.QuerySelector("#corePrice_feature_div > div > span > span.a-offscreen").InnerText.Replace("TL", String.Empty));

            if (price == 0)
            {
                return new Response(ResponseCode.Error, "Product has not a price.");
            }

            using var trackedProduct = new TrackedProduct();
            trackedProduct.ProductId = product.Id;
            trackedProduct.Interval = addTrackingProductDto.Interval;
            trackedProduct.CurrentPrice = Math.Round(price, 2);
            trackedProduct.TargetPrice = addTrackingProductDto.TargetPrice;
            trackedProduct.PriceHistory = [String.Format("{0}-{1}", DateTime.Now.ToString(), price.ToString())];
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

            return null;

        }


        public async Task<Response> DeleteTrackingProduct(int productId)
        {
            try
            {
                var trackingProduct = await _trackedProductReadRepository.GetSingleAsync(x => x.ProductId == productId);

                if (trackingProduct == null)
                {
                    return new Response(ResponseCode.NotFound, "Tracking product not found");
                }

                Remove(trackingProduct);
                int state = await SaveChangesAsync();

                if (state > 0)
                {
                    return new Response(ResponseCode.Success, "Product deleted from tracking list succesfully");
                }
                else
                {
                    return new Response(ResponseCode.Error, "Error on deleting");
                }
            }
            catch (Exception)
            {
                return new Response(ResponseCode.Error, "Error on deleting");
            }
            
        }

        public async Task<Response> UpdateTrackedProductIntervalAndTargetPrice  (TrackingProductPriceAndIntervalDto trackingProductPriceAndIntervalDto)
        {
            try
            {
                var entity = await _trackedProductReadRepository.GetSingleAsync(x => x.ProductId == trackingProductPriceAndIntervalDto.ProductId, true);
                if (entity == null)
                {
                    return new Response(ResponseCode.NotFound, "Product not found.");
                }

                entity.TargetPrice = trackingProductPriceAndIntervalDto.TargetPrice;
                entity.Interval = trackingProductPriceAndIntervalDto.Interval;

                await _trackedProductReadRepository.SaveChangesAsync();

                return new Response(ResponseCode.Success, "Tracked product target price and interval time updated.");
            }
            catch (Exception)
            {
                return new Response(ResponseCode.Error, "Error on updating");
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
