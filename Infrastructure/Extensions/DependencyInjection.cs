using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repos;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserRelationshipService, UserRelationshipService>();
            services.AddScoped<IUserRelationshipRepo, UserRelationshipRepo>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ITokenService, TokenService>();
            services.AddHostedService<TokenCleanupService>();
            services.AddSingleton<IFileStorageService, FileStorageService>();
            services.AddSingleton<IFileValidatorService, FileValidatorService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddTransient<IUrlService, UrlService>();

            return services;
        }

        
    }
}
