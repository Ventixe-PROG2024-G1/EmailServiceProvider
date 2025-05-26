using Azure;
using Azure.Communication.Email;
using EmailServiceProvider.DTOs;
using EmailServiceProvider.Models;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace EmailServiceProvider.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailClient _client;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _client = new EmailClient(_configuration["ACS:Connectionstring"]);
        }

        public async Task<EmailResponse> SendAsync(EmailMessageDTO request)
        {
            var senderAddress = _configuration["ACS:SenderAddress"];

            try
            {
                var recipients = request.Recipients.Select(email => new EmailAddress(email)).ToList();
                var emailMessage = new EmailMessage(
                    senderAddress: senderAddress,
                    content: new EmailContent(request.Subject)
                    {
                        PlainText = request.PlainText,
                        Html = request.Html
                    },
                    recipients: new EmailRecipients(recipients));

                var response = await _client.SendAsync(Azure.WaitUntil.Completed, emailMessage);

                return new EmailResponse
                {
                    Succeeded = response.HasCompleted,
                    Error = response.HasCompleted ? "" : "Email send-confirmation couldn't be confirmed"
                };
            }
            catch (Exception ex)
            {
                return new EmailResponse { Succeeded = false, Error = $"An error occurred: {ex.Message}" };
            }
        }
    }
}