using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace AuthApp.Service
{
    public class MailJetEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetOptions _mailJetOptions;
        public MailJetEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _mailJetOptions = _configuration.GetSection("MailJet").Get<MailJetOptions>();

            MailjetClient client = new MailjetClient(_mailJetOptions.ApiKey, _mailJetOptions.SecretKey);
    
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }

            .Property(Send.Messages, new JArray {
                new JObject {
                {
                    "From",
                    new JObject {
                        {"Email", "savior_gildo.21@proton.me"},
                        {"Name", "Gildo"}
                    }
                }, {
                "To",
                new JArray {
                    new JObject {
                        {
                            "Email",
                            email
                        }, {
                            "Name",
                            "Gildo"
                        }
                    }
                }
                }, {
                        "Subject",
                        subject
                }, {
                    "HTMLPart",
                    htmlMessage
                },

            }});

            await client.PostAsync(request);
        }
    }
}
