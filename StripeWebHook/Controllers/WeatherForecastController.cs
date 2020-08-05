using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Stripe;

namespace StripeWebHook.Controllers
{
    [ApiController]
    public class WeatherForecastController : Controller
    {
        [Route("/")]
        public IActionResult Default()
        {
            return Ok(new { message = "Tested."});
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private readonly ISendGridClient _sendgridClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config, ISendGridClient sendgridClient )
        {
            _logger = logger;
            _config = config;
            _sendgridClient = sendgridClient;
        }

        [Route("Webhook")]
        [HttpPost]
        public async Task<IActionResult> WebHookEndpoint()
        {
            StripeConfiguration.ApiKey = _config["stripe-secret-key"];

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var response = SendEmail(json);

            return Ok(new { json1 = response });
        }

        [Route("SendEmail")]
        [HttpGet]
        public async Task<IActionResult> SendEmail()
        {
            var from = new EmailAddress("umesh.kumawat@rsystems.com", "Example User");
            var to = new EmailAddress("ukumawat@gmail.com", "Example User");
            var msg = new SendGridMessage
            {
                From = from,
                Subject = "Sending with Twilio SendGrid is Fun"
            };
            msg.AddContent(MimeType.Text, "and easy to do anywhere, even with C#");
            msg.AddTo(to);

            var response = await _sendgridClient.SendEmailAsync(msg);

            return Ok(new { response = response });
        }

        private async Task<SendGrid.Response> SendEmail(string message)
        {
            var from = new EmailAddress("umesh.kumawat@rsystems.com", "Example User");
            var to = new EmailAddress("ukumawat@gmail.com", "Example User");
            var msg = new SendGridMessage
            {
                From = from,
                Subject = "Sending with Twilio SendGrid is Fun"
            };
            msg.AddContent(MimeType.Text, message);
            msg.AddTo(to);

            var response = await _sendgridClient.SendEmailAsync(msg);

            return response;
        }

    }
}
