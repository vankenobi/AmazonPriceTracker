using AmazonPriceTrackerAPI.Application.Repositories;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Entities.Dto;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Amazon_Price_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackedProductsController : ControllerBase
    {
        private readonly ITrackedProductWriteRepository _trackedProductWriteRepository;
        private readonly ITrackedProductReadRepository _trackedProductReadRepository;

        public TrackedProductsController(ITrackedProductWriteRepository trackedProductWriteRepository,
                                         ITrackedProductReadRepository trackedProductReadRepository)
        {
            _trackedProductWriteRepository = trackedProductWriteRepository;
            _trackedProductReadRepository = trackedProductReadRepository;
        }

        [HttpPost]
        [Route("AddProductTracking")]
        public async Task<Response> AddProductTracking(AddProductTrackingDto addProductTrackingDto)
        {
            return await _trackedProductWriteRepository.AddProductTracking(addProductTrackingDto);
        }

        [HttpGet]
        [Route("GetAllTrackedProduct")]
        public async Task<Response<List<TrackedProduct>>> GetAllTrackedProduct() {
            return await _trackedProductReadRepository.GetAllTrackedProducts();
        }



        [HttpGet]
        [Route("deneme")]
        public  string deneme()
        {
            return "deneme";
        }


    }
}
