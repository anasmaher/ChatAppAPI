using Domain.Entities;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IEmailService
    {
        void SendEmailAsync(EmailMetadata emailMetadata);
    }
}
