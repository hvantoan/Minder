using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Minder.Exceptions;
using Minder.Service.Configurations;
using Minder.Service.Helpers;
using Minder.Service.Interfaces;
using Minder.Services.Common;
using Minder.Services.Models.User;
using Minder.Services.Resources;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class EmailService : BaseService, IEmailService {
        private readonly ICacheManager cacheManager;

        public EmailService(IServiceProvider serviceProvider, ICacheManager cacheManager) : base(serviceProvider) {
            this.cacheManager = cacheManager;
        }

        public async Task<bool> SendOTP(UserDto user) {
            var otp = await this.cacheManager.CreateOrUpdate(user.Username!);
            var body = EMailHelper.GetOTPBody(otp);
            return await Send($"[Minder]XÁC NHẬN NGƯỜI DÙNG {user.Name}", user.Username!, body);
        }

        private async Task<bool> Send(string subject, string to, string body) {
            var mailSetting = this.configuration.GetSection("MailSetting").Get<MailSetting>()!;

            var message = new MailMessage() {
                From = new MailAddress(mailSetting.From),
                Subject = subject,
                IsBodyHtml = true,
                Body = body,
            };
            message.To.Add(new MailAddress(to));

            var smtpClient = new SmtpClient() {
                UseDefaultCredentials = false,
                Host = mailSetting.Host,
                Port = Convert.ToInt32(mailSetting.Port),
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(mailSetting.UserName, mailSetting.Password)
            };

            smtpClient.Send(message);

            return await Task.FromResult(true);
        }
    }
}