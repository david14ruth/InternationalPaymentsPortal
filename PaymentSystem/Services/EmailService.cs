using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PaymentSystem.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // 🔥 FIX: changed from void → Task
        public Task SendOtp(string toEmail, string otp)
        {
            var email = _config["Gmail:Email"];
            var appPassword = _config["Gmail:AppPassword"];

            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(email, appPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(email!, "GlobalTrust Bank"),
                Subject = "Your OTP Code",
                Body = $@"
                <div style='font-family:Arial;text-align:center'>
                    <h2 style='color:#0d6efd'>GlobalTrust Bank OTP</h2>
                    <h1 style='letter-spacing:6px'>{otp}</h1>
                    <p>Do not share this code.</p>
                </div>",
                IsBodyHtml = true
            };

            mail.To.Add(toEmail!);

            client.Send(mail);

            return Task.CompletedTask;
        }
    }
}