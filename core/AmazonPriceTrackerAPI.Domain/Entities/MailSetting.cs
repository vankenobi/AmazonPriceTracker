using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Domain.Entities
{
    [NotMapped]
    public class MailSetting : IDisposable
    {
        public short Port { get; set; }
        public string Host { get; set; }
        public bool EnableSSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void Dispose()
        {
                
        }
    }
}
