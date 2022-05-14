using AmazonPriceTrackerAPI.Application.Repositories;
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
        public TrackedProductsController(ITrackedProductWriteRepository trackedProductWriteRepository)
        {
            _trackedProductWriteRepository = trackedProductWriteRepository;
        }

        [HttpPost]
        [Route("AddProductTracking")]
        public async Task<Response> AddProductTracking(AddProductTrackingDto addProductTrackingDto)
        {
            return await _trackedProductWriteRepository.AddProductTracking(addProductTrackingDto);
        }

        
    }
}
