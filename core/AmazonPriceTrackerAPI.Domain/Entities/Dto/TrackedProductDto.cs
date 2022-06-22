using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities.Dto
{
    public class TrackedProductDto : IDisposable
    {
        public int ProductId { get; set; }

        public int ProductTrackedId { get; set; }

        public string Url { get; set; }

        public string ProductName { get; set; }

        public string StockState { get; set; }

        public string? Image { get; set; }

        public double? Rate { get; set; }

        public string? Description { get; set; }
        
        public double CurrentPrice { get; set; }

        public double TargetPrice { get; set; }

        public int Interval { get; set; }

        public string[] PriceHistory { get; set; }

        public DateTime NextRunTime { get; set; }

        public DateTime MailSendingDate { get; set; }

        public List<string>? TechnicalDetails { get; set; }

        public double PriceChange { get; set; }

        public void Dispose() {}
    }
}
