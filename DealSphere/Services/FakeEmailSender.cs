using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace DealSphere.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"[FAKE EMAIL] To: {email}, Subject: {subject}");
            return Task.CompletedTask;
        }
    }
}
