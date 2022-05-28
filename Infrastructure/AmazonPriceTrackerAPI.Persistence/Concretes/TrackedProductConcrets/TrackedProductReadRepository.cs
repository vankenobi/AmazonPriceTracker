using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
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
        private readonly IProductReadRepository _productReadRepository;

        public TrackedProductReadRepository(AmazonPriceTrackerDbContext context, IProductReadRepository productReadRepository) : base(context)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<Array> GetLastPricesByProductId(int id) 
        {
            var priceHistory = await GetByIdAsync(id);
            return priceHistory.PriceHistory;
        }

        public async Task<Response<List<TrackedProductDto>>> GetAllTrackedProducts() 
        {
            try
            { 
                var products = await _productReadRepository.GetAllAsync();
                var trackedProducts = await GetAllAsync();

                var result = (from p in products
                             join t in trackedProducts on p.Id equals t.ProductId
                             select new TrackedProductDto() { CurrentPrice = t.CurrentPrice,
                                                              Description = p.Description,
                                                              Image = p.Image,
                                                              ProductName = p.Name,
                                                              Interval = t.Interval,
                                                              ProductTrackedId = t.Id,
                                                              PriceChange = t.PriceChange,
                                                              Rate = p.Rate,
                                                              PriceHistory = t.PriceHistory,
                                                              StockState = p.StockState,
                                                              MailSendingDate = t.MailSendingDate,
                                                              TargetPrice = t.TargetPrice,
                                                              NextRunTime = t.NextRunTime,
                                                              ProductId = p.Id,
                                                              Url = p.Url,
                                                              TechnicalDetails = p.TechnicalDetails}).ToList();
                             

                return new Response<List<TrackedProductDto>>(ResponseCode.Success, result);
            }
            catch (Exception)
            {
                return new Response<List<TrackedProductDto>>(ResponseCode.Error,"Error on get all tracked products");
            }
        }


        
        
    }
}
