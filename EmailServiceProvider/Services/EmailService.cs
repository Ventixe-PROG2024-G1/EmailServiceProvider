using Azure;
using Azure.Communication.Email;
using EmailServiceProvider.DTOs;
using EmailServiceProvider.Models;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace EmailServiceProvider.Services
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmail(EmailMessageRequest request, ServerCallContext context);
    }

    public class EmailService(EmailClient client, IOptions<AzureCommunicationSettings> options)
         : EmailContract.EmailContractBase, IEmailService
    {
        private readonly EmailClient _client = client;
        private readonly AzureCommunicationSettings _settings = options.Value;

        public override async Task<EmailResponse> SendEmail(EmailMessageRequest request, ServerCallContext context)
        {
            try
            {
                var recipients = request.Recipients.Select(email => new EmailAddress(email)).ToList();
                var emailMessage = new EmailMessage(
                    senderAddress: _settings.SenderAddress,
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