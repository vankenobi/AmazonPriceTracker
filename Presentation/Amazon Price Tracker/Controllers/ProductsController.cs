using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using Microsoft.AspNetCore.Mvc;

namespace Amazon_Price_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;

        public ProductsController(IProductWriteRepository productWriteRepository,
                                  IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        [HttpPost]
        [Route("AddNewProductWithUrlAsync")]
        public async Task<Response> AddNewProductWithUrl([FromBody] string url)
        {
            return await _productWriteRepository.AddNewProductWithUrlAsync(url);
        }

        [HttpPost]
        [Route("DeleteProductAsync")]
        public async Task<Response> DeleteTheProduct([FromBody] int id)
        {
            return await _productWriteRepository.DeleteProductAsync(id);
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<Response<List<Product>>> GetAllProducts() {
            return await _productReadRepository.GetAllProductsAsync();
        }

        [HttpPut]
        [Route("EditFavoriteStateAsync")]
        public async Task<Response> EditFavoriteStateAsync([FromBody] int productId) 
        {
            return await _productWriteRepository.ChangeFavoriteStateAsync(productId);
        }


    }
}
