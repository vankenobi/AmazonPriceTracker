using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AmazonPriceTrackerAPI.Application.Repositories.MailRepo
{
    public interface IMailRepository
    {
        void SetMailSettings(MailSetting mailSetting);
        Task<Response> SendAnEmailAsync(MailMessage mailMessage);
        void ReadMailSettings();
    }
}
