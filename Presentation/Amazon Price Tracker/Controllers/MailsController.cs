using AmazonPriceTrackerAPI.Application.Repositories.MailRepo;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Amazon_Price_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly IMailRepository _mailRepository;

        public MailsController(IMailRepository mailRepository)
        {
            _mailRepository = mailRepository;
        }

        [HttpPost]
        [Route("SetMailServerSettings")]
        public Response Set([FromBody]MailSetting mailSetting) 
        {
            _mailRepository.SetMailSettings(mailSetting);
            return new Response(ResponseCode.Success);   
        }
    }
}
