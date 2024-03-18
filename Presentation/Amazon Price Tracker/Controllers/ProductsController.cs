using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Context;

namespace Amazon_Price_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly Serilog.ILogger _logger;

        public ProductsController(IProductWriteRepository productWriteRepository,
                                  IProductReadRepository productReadRepository,
                                  Serilog.ILogger logger)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _logger = logger;
        }

        [HttpPost]
        [Route("AddNewProductWithUrlAsync")]
        public async Task<Response> AddNewProductWithUrl([FromBody] string url)
        {
            var requestId = LogContext.PushProperty("RequestId", Guid.NewGuid());
            _logger.Information("Processing request {RequestId}", requestId);
            return await _productWriteRepository.AddNewProductWithUrlAsync(url);
        }

        [HttpPost]
        [Route("DeleteProductAsync")]
        public async Task<Response> DeleteTheProduct([FromBody] int id)
        {
            var requestId = LogContext.PushProperty("RequestId", Guid.NewGuid());
            _logger.Information("Processing request {RequestId}", requestId);
            return await _productWriteRepository.DeleteProductAsync(id);
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<Response<List<Product>>> GetAllProducts() {
            var requestId = LogContext.PushProperty("RequestId", Guid.NewGuid());
            _logger.Information("Processing request {RequestId}", requestId);
            return await _productReadRepository.GetAllProductsAsync();
        }

        [HttpPut]
        [Route("EditFavoriteStateAsync")]
        public async Task<Response> EditFavoriteStateAsync([FromBody] int productId) 
        {
            var requestId = LogContext.PushProperty("RequestId", Guid.NewGuid());
            _logger.Information("Processing request {RequestId}", requestId);
            return await _productWriteRepository.ChangeFavoriteStateAsync(productId);
        }


    }
}
