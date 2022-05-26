using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;  
using AmazonPriceTrackerAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Persistence.Concretes.TrackedProductConcrets
{
    public class TrackedProductReadRepository : ReadRepository<TrackedProduct>, ITrackedProductReadRepository
    {
        public TrackedProductReadRepository(AmazonPriceTrackerDbContext context) : base(context)
        {
        }

        public async Task<Array> GetLastPricesByProductId(int id) 
        {
            var priceHistory = await GetByIdAsync(id);
            return priceHistory.PriceHistory;
        }

        public async Task<Response<List<TrackedProduct>>> GetAllTrackedProducts() 
        {
            try
            {
                List<TrackedProduct> trackedProducts = await GetAllAsync();
                return new Response<List<TrackedProduct>>(ResponseCode.Success, trackedProducts);
            }
            catch (Exception)
            {
                return new Response<List<TrackedProduct>>(ResponseCode.Error,"Error on get all tracked products");
            }
        }


        
        
    }
}
