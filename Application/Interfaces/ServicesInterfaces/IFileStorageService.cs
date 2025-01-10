using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);

        Task DeleteFile(string path);
    }
}
