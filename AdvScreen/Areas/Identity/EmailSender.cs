using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AdvScreen.Areas.Identity
{
    public class EmailSender : IEmailSender
    {
        // Our private configuration variables
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;

        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
        }


        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", userName));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(host, port, enableSSL);
                    await client.AuthenticateAsync(userName, password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception e)
                { 
                }

                await client.DisconnectAsync(true);
            }
        }
        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync2(string email, string subject, string htmlMessage)
        {
            var client = new System.Net.Mail.SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSSL
            };
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(
                userName
                , password
                );
            var mm = new MailMessage(userName, email, subject, htmlMessage) { IsBodyHtml = true };
            mm.From = new MailAddress(userName, "Комманда");

            var test = client.SendMailAsync(
                mm
            );
            return test;

        }
    }
}
