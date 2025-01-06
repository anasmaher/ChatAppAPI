using Application;
using Application.Mappings;
using ChatAppAPI.Filters;
using ChatAppAPI.Mappings;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using YourProject.WebAPI.Filters;

namespace ChatAppAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = builder.Configuration.GetSection("JWT");

            // Add services to the container.
            builder.Services.AddApplicationServices();

            builder.Services.AddInfrastructureServices();

            builder.Services.AddAuthenticationServices(builder.Configuration);
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GenericResponseFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Collect validation errors
                    string? errors = (context.ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .Select(ms => ms.Value.Errors.Select(e => e.ErrorMessage))).ElementAt(0).ElementAt(0);

                    // Create your custom response
                    var genericResponse = new
                    {
                        success = false,
                        data = (object)null,
                        message = "Validation failed.",
                        errors = errors
                    };

                    return new BadRequestObjectResult(genericResponse);
                };
            })
            .AddNewtonsoftJson(opts =>
                opts.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAuthorization();

            builder.Services.AddAutoMapper(typeof(ViewModelToDtoProfile));
            builder.Services.AddAutoMapper(typeof(DtoToEntityProfile));

            // Add caching
            builder.Services.AddOutputCache();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(3);  // Token is valid for 3 hours
            });

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 5 * 1024 * 1024; // 5 MB
            });

            // Configure CORS to allow specific origin and credentials
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins("")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials(); // Allow cookies
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerDocumentation();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                await SeedRolesAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.DisplayRequestDuration();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin"); // Apply the CORS policy

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOutputCache();

            app.MapControllers();

            app.Run();
        }

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

    }
}
