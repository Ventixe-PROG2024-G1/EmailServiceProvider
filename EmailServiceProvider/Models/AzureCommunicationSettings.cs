namespace EmailServiceProvider.Models
{
    public class AzureCommunicationSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string SenderAddress { get; set; } = null!;
    }
}