using Application.Interfaces.ServicesInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT");

            // Clear default claims mapping
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                // Set default schemes for authentication and challenge
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Add JWT Bearer authentication
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        Environment.GetEnvironmentVariable("Key")
                        ?? throw new InvalidOperationException("JWT signing key not found in environment variables."))),
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        // Token validation logic
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var userId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                        var tokenVersionClaim = context.Principal.FindFirst("TokenVersion")?.Value;

                        if (!int.TryParse(tokenVersionClaim, out var tokenVersion))
                        {
                            context.Fail("Invalid token version.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user == null || user.TokenVersion != tokenVersion)
                        {
                            context.Fail("Token is no longer valid.");
                            return;
                        }

                        // Check if the token has been revoked (single logout)
                        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                        var jti = context.Principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

                        if (string.IsNullOrEmpty(jti))
                        {
                            context.Fail("Invalid token.");
                            return;
                        }

                        var isRevoked = await tokenService.IsTokenRevokedAsync(jti);
                        if (isRevoked)
                        {
                            context.Fail("Token has been revoked.");
                            return;
                        }
                    }
                };
            })
            // Add Cookie authentication for external login (Google)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/api/Account/LoginGoogle"; // Set the login path for initiating Google login
                options.Events.OnRedirectToLogin = context =>
                {
                    // Return 401 instead of redirecting to the login page
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    // Return 403 instead of redirecting
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("ClientId")
                    ?? throw new InvalidOperationException("Google ClientId not found in environment variables.");
                options.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret")
                    ?? throw new InvalidOperationException("Google ClientSecret not found in environment variables.");

                options.Scope.Add("profile");
                options.SaveTokens = true;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookies for sign-in
            });

            services.AddAuthorization();

            return services;
        }
    }
}
