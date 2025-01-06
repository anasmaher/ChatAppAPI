using Application.DTOs.ResultsDTOs;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface ITokenService
    {
        Task<AuthenticationResult> GenerateTokenAsync(AppUser user, bool generateRefreshToken = true);

        Task StoreTokenAsync(string userId, string tokenId, DateTime expirationTime);

        Task RevokeTokenAsync(string tokenId);

        Task<bool> IsTokenRevokedAsync(string tokenId);

        Task CleanupExpiredTokensAsync();

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
