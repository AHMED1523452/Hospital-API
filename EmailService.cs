using FluentEmail.Core;

namespace Hospital_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            this.fluentEmail = fluentEmail;
        }

        public async Task SendEmail(string toEmail, string Subject, string Body)
        {

            var emailResult = await fluentEmail.
                To(toEmail)
                .Subject(Subject)
                .Body(Body, isHtml: true)
                .SendAsync();
        }
    }
}
