using AmazonPriceTrackerAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities
{
    public class TrackedProduct : BaseEntity, IDisposable
    {

        public int ProductId { get; set; }

        public double CurrentPrice { get; set; }

        public double TargetPrice { get; set; }

        public int Interval { get; set; }

        public string [] PriceHistory { get; set; }

        public DateTime NextRunTime { get; set; }

        public DateTime MailSendingDate { get; set; }

        public double PriceChange { get; set; }

        public void Dispose() 
        {
            Console.WriteLine("Tracked Product dispose runned.");
        }
    }
}
