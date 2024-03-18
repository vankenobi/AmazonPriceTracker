using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Persistence.Contexts;

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
                var products = await GetAllAsync();
                var sortedProducts = products.OrderBy(x => x.Name).ToList();
                return new Response<List<Product>>(ResponseCode.Success, sortedProducts);
            }
            catch (Exception ex)
            {
                return new Response<List<Product>>(ResponseCode.Error, $"Error on get all products: {ex.Message}");
            }
        }
    }
}
