﻿using AmazonPriceTrackerAPI.Domain.Entities.Common;

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
