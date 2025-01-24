using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Infrastructure.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Basic Swagger Doc setup
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ChatApp API",
                    Version = "v1",
                    Description = "An API for ChatApp",
                    Contact = new OpenApiContact
                    {
                        Name = "Anas Al-Horigy",
                        Email = "anas.elhorigy@gmail.com",
                    }
                });

                // Security Definition
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token below.\r\n\r\nExample: \"eyJhb...\""
                });

                // Security Requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Include XML comments from multiple projects
                var xmlFiles = new[]
                {
                    "ChatAppAPI.xml",
                };

                var basePath = AppContext.BaseDirectory;

                foreach (var xmlFile in xmlFiles)
                {
                    var xmlPath = Path.Combine(basePath, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
            });

            return services;
        }
    }
}