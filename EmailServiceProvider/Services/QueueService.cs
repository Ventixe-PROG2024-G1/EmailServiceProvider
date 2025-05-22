using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;

namespace EmailServiceProvider.Services;

public class QueueService
{
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;

    public QueueService(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
        _client = new ServiceBusClient(_configuration["ASB:ConnectionString"]);
        _processor = _client.CreateProcessor(_configuration["ASB:QueueName"]);
    }

    public async Task HandleMessagesAsync()
    {
        _processor.ProcessMessageAsync += async args =>
        {
            var body = args.Message.Body.ToString();
            var request = JsonConvert.DeserializeObject<EmailMessageRequest>(body);

            var result = await _emailService.SendAsync(request);
            if (result.Succeeded)
            {
                await args.CompleteMessageAsync(args.Message);
            }
            else
            {
                await args.DeadLetterMessageAsync(args.Message);
            }
        };

        _processor.ProcessErrorAsync += args =>
        {
            return Task.CompletedTask;
        };

        await _processor.StartProcessingAsync();
    }
}