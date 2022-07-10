using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using AmazonPriceTrackerAPI.Application.Repositories.MailRepo;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using System.Net;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using Microsoft.Extensions.DependencyInjection;

namespace AmazonPriceTrackerAPI.Infrastructure.Concrets
{
    public class MailRepository : SmtpClient, IMailRepository
    {
        private readonly SmtpClient _smtpClient;

        public MailRepository()
        {
            _smtpClient = new SmtpClient();
            ReadMailSettings();
        }

        public void SetMailSettings(MailSetting mailSetting)
        {
            var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory() + "../../../Presentation/Amazon Price Tracker", "appsettings.json");
            var json = File.ReadAllText(appSettingsPath);

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);

            config.EmailSettings.Port = mailSetting.Port;
            config.EmailSettings.Host = mailSetting.Host;
            config.EmailSettings.EnableSsl = mailSetting.EnableSSL;
            config.EmailSettings.Credentials.Username = mailSetting.Username;
            config.EmailSettings.Credentials.Password = mailSetting.Password;

            var newJson = JsonConvert.SerializeObject(config, Formatting.Indented, jsonSettings);
            File.WriteAllText(appSettingsPath, newJson);
        }

        public void ReadMailSettings()
        {
            //var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory() + "../../../Presentation/Amazon Price Tracker", "appsettings.json");
            var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
            var json = File.ReadAllText(appSettingsPath);

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());

            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(json, jsonSettings);
          
            _smtpClient.Port = (int)(config.EmailSettings.Port);
            _smtpClient.Host = (string)(config.EmailSettings.Host);
            _smtpClient.EnableSsl = (bool)(config.EmailSettings.EnableSsl);
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.Credentials = new NetworkCredential((string)(config.EmailSettings.Credentials.Username), (string)(config.EmailSettings.Credentials.Password));
        }   

        public async Task<Response> SendAnEmailAsync(MailMessage mailMessage)
        {
            await _smtpClient.SendMailAsync(mailMessage);
            return new Response(ResponseCode.Success, "Mail başarılı bir şekilde gönderildi.");
        }
        


    }
}
