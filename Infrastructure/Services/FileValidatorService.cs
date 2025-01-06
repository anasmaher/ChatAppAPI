using Application.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class FileValidatorService : IFileValidatorService
    {
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".jpg",  new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
            { ".png",  new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47 } } },
            { ".gif",  new List<byte[]> { new byte[] { 0x47, 0x49, 0x46 } } },
        };

        public bool IsValidFileSignature(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension) || !_fileSignature.ContainsKey(extension))
            {
                return false;
            }

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var headerBytes = reader.ReadBytes(_fileSignature[extension][0].Length);

                return _fileSignature[extension].Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
