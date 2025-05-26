namespace EmailServiceProvider.DTOs
{
    public class EmailMessageDTO
    {
        public List<string> Recipients { get; set; } = null!;

        public string? Subject { get; set; }

        public string? PlainText { get; set; }

        public string Html { get; set; } = null!;
    }
}