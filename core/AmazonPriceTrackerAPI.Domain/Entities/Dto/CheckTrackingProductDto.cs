using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities.Dto
{
    public class CheckTrackingProductDto
    {
        public int TrackingId { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; }
    }
}
