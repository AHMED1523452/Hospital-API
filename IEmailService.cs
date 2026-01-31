namespace Hospital_API.Services
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string Subject, string Body);
    }
}
