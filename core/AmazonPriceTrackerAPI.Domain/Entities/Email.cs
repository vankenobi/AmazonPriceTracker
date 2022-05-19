using AmazonPriceTrackerAPI.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities
{
    public class Email : BaseEntity
    {
        public string Name { get; set; }
        public string MailAddress { get; set; }
    }
}
