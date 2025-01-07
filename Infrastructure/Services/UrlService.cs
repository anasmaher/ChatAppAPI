using Application.Interfaces.ServicesInterfaces;
using static System.Net.WebRequestMethods;

namespace Infrastructure.Services
{
    public class UrlService : IUrlService
    {
        public string GenerateResetPasswordUrl(string token)
        {
            var clientAppUrl = "http://192.168.1.7:5000";
            string encodedToken = Uri.EscapeDataString(token);
            return $"{clientAppUrl}/reset-password?token={encodedToken}";
        }
    }
}
