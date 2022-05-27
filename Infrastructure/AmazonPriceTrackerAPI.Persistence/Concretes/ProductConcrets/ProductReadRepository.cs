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

namespace AmazonPriceTrackerAPI.Persistence.Concretes
{
    public class ProductReadRepository : ReadRepository<Product>, IProductReadRepository
    {
        public ProductReadRepository(AmazonPriceTrackerDbContext context) : base(context)
        {
        }

        public async Task<Response<List<Product>>> GetAllProductsAsync()
        {
            try
            {
                List<Product> products = await GetAllAsync();
                return new Response<List<Product>>(ResponseCode.Success, products);
            }
            catch (Exception)
            {
                return new Response<List<Product>>(ResponseCode.Error, "Error on get all products");
            }
        }
    }
}
