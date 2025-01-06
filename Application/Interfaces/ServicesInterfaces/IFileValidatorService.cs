using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IFileValidatorService
    {
        public bool IsValidFileSignature(IFormFile file);
    }
}
