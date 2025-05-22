using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailServiceProvider.Models;
using EmailServiceProvider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddOpenApi();

// Konfigurera Azure Communication Service
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<QueueService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Starta bakgrundstj�nst f�r att hantera meddelanden fr�n k�n
using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider.GetRequiredService<QueueService>();
    await service.HandleMessagesAsync();
}

app.Run();