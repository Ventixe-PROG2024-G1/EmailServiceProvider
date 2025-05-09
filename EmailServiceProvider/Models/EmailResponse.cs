namespace EmailServiceProvider.Models
{
    public class EmailResponse
    {
        public bool Succeeded { get; set; }
        public string? Error { get; set; }
    }

    public class EmailServiceResult<T> : EmailResponse
    {
        public T? Result { get; set; }
    }
}