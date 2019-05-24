using System.Threading.Tasks;

namespace Boilerplate.Services.Abstractions
{
    public interface IEmailService
    {
        Task SendMail(string to, string subject, string message);
    }
}
