using AmazonPriceTrackerAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities
{
    public class Product : BaseEntity, IDisposable
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string StockState { get; set; }

        public string? Image { get; set; }

        public double? CurrentPrice { get; set; }

        public double? Rate { get; set; }

        public string? Description { get; set; }

        public bool isTracking { get; set; } = false;

        public List<string>? TechnicalDetails { get; set; }

        public void Dispose() {
            Console.WriteLine("Dispose worked.");
        }
    }
}
