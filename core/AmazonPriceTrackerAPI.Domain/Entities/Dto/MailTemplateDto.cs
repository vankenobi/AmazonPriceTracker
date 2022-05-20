using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities.Dto
{
    public class MailTemplateDto : IDisposable
    {
        public double CurrentPrice { get; set; }
        public double OldPrice { get; set; }
        public double Discount { get; set; }
        public string ImagePath  { get; set; }
        public string Title { get; set; }

        public void Dispose() 
        {}
    }
}
