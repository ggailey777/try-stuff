using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using SendGrid;
using System.Collections.Generic;
using SendGrid.Helpers.Mail;

namespace webbish6_site_monitor
{
    public static class Monitor
    {
        static readonly string siteUrl = System.Environment.GetEnvironmentVariable("site-url");
        static readonly string apiKey = System.Environment.GetEnvironmentVariable("SendGridKey");
        private static string message = string.Empty;

        [FunctionName("MonitorSiteUptime")]
        public static async Task Run([TimerTrigger("%ScheduleAppSetting%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await client.GetAsync(siteUrl);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    log.LogError(@"Site is down: \n{0}", httpResponseMessage);

                    await SendMail(string.Format("Your site '{0}' is {1}!", siteUrl, "down"));
                }
            }
        }
        private static async Task SendMail(string message)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("ggailey777@hotmail.com", "Webbish6.com monitor");
            var to = new EmailAddress("glenga@microsoft.com", "Glenn Gailey");
            var subject = siteUrl + "monitor alert";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, "");
            var response = await client.SendEmailAsync(msg);
        }
        //private static async Task SendText(string message)
        //{
            // Do Twilio client stuff here

        //}
    }
}
