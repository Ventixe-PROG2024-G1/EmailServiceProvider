using Azure.Communication.Email;
using EmailServiceProvider.Models;
using EmailServiceProvider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddGrpc();
builder.Services.AddSingleton(x => new EmailClient(builder.Configuration["ACS:ConnectionString"]));
builder.Services.Configure<AzureCommunicationSettings>(builder.Configuration.GetSection("ACS"));
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGrpcService<EmailService>();
app.MapGet("/", () => "EmailServiceProvider is running.");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();