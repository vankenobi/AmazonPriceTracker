using AmazonPriceTrackerAPI.Application.Repositories.MailRepo;
using AmazonPriceTrackerAPI.Domain.Entities;
using AmazonPriceTrackerAPI.Domain.Shared.ComplexTypes;
using AmazonPriceTrackerAPI.Domain.Shared.Concret;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace Amazon_Price_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly IMailRepository _mailRepository;

        public MailsController(IMailRepository mailRepository)
        {
            _mailRepository = mailRepository;
        }

        [HttpPost]
        [Route("SetMailServerSettings")]
        public Response Set([FromBody]MailSetting mailSetting) 
        {
            _mailRepository.SetMailSettings(mailSetting);
            return new Response(ResponseCode.Success);   
        }

        [HttpGet]
        [Route("EditFavoriteStateAsync")]
        public void SendMailTemp()
        {
            //var smtpClient = new SmtpClient("localhost", 25);

            //smtpClient.Credentials = new NetworkCredential(
            //    "amazonpricetracker823@gmail.com", "$@r5!Q2$e9");

            //smtpClient.EnableSsl = false;

            //var mailMessage = new MailMessage();

            //mailMessage.From = new MailAddress("amazonpricetracker823@gmail.com");
            //mailMessage.To.Add("musakucuk99@gmail.com");
            //mailMessage.Subject = "Test Email";
            //mailMessage.Body = "This is a test email.";

            //smtpClient.Send(mailMessage);

            //Console.WriteLine("Mail sent successfully!");

            // Gmail SMTP ayarları
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            // Gmail hesap bilgileri
            string emailFrom = "amazonpricetracker823@gmail.com";
            string password = "$@r5!Q2$e9";

            // Gönderen ve alıcı e-posta adresleri
            MailAddress fromAddress = new MailAddress(emailFrom);
            MailAddress toAddress = new MailAddress("musakucuk99@gmail.com");

            // Mail nesnesi oluştur
            MailMessage message = new MailMessage(fromAddress, toAddress);
            message.Subject = "Deneme";
            message.Body = "Bu bir deneme mesajıdır";
            message.IsBodyHtml = true;

            // SMTP istemci ayarları
            SmtpClient smtpClient = new SmtpClient(smtpAddress, portNumber);
            smtpClient.EnableSsl = enableSSL;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new NetworkCredential(emailFrom, password);

            // E-postayı gönder
            smtpClient.Send(message);
        }
    }
}
