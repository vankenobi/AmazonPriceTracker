using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities.Dto
{
    public class TrackingProductPriceAndIntervalDto
    {
        public int ProductId { get; set; }

        public double TargetPrice { get; set; }

        public int Interval { get; set; }
    }
}
