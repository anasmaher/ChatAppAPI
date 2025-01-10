using Application.Interfaces.ServicesInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Extensions
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWT");

            // Read the signing key from environment variables
            var signingKey = Environment.GetEnvironmentVariable("Key")
                ?? throw new InvalidOperationException("JWT signing key not found in environment variables.");

            // Ensure that the default claim type mappings are preserved
            // Do not clear the default inbound claim type map

            // Configure JWT Bearer authentication
            services.AddAuthentication(options =>
            {
                // Set default schemes for authentication and challenge
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Add JWT Bearer authentication
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    NameClaimType = ClaimTypes.NameIdentifier, // Ensure standard claim types are used
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

                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Fail("User ID claim not found.");
                            return;
                        }

                        if (!int.TryParse(tokenVersionClaim, out var tokenVersion))
                        {
                            context.Fail("Invalid token version.");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user is null || user.TokenVersion != tokenVersion)
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
                    },

                    // Custom response for unauthorized access (401)
                    OnChallenge = async context =>
                    {
                        // Skip the default logic.
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var genericResponse = new
                        {
                            success = false,
                            data = (object)null,
                            message = "Unauthorized access.",
                            errors = "User is not authenticated."
                        };

                        var json = JsonSerializer.Serialize(genericResponse);
                        await context.Response.WriteAsync(json);
                    },

                    // Custom response for forbidden access (403)
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var genericResponse = new
                        {
                            success = false,
                            data = (object)null,
                            message = "Forbidden access.",
                            errors = "User does not have permission to access this resource."
                        };

                        var json = JsonSerializer.Serialize(genericResponse);
                        await context.Response.WriteAsync(json);
                    }
                };
            })
            // Add Cookie authentication for external login (Google)
            .AddCookie("ExternalCookie", options =>
            {
                options.LoginPath = "/api/ExternalAuth/GoogleLogin"; // Set the login path for initiating Google login
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
                // Read Google ClientId and ClientSecret from environment variables
                options.ClientId = Environment.GetEnvironmentVariable("ClientId")
                    ?? throw new InvalidOperationException("Google ClientId not found in environment variables.");

                options.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret")
                    ?? throw new InvalidOperationException("Google ClientSecret not found in environment variables.");

                options.Scope.Add("profile");
                options.SaveTokens = true;
                options.SignInScheme = "ExternalCookie"; // Use the external cookie scheme for sign-in
            });

            services.AddAuthorization();

            return services;
        }
    }
}