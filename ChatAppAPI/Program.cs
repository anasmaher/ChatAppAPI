using Application.Mappings;
using ChatAppAPI.Mappings;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatApp.ChatAppAPI.Filters;
using ChatAppAPI.Hubs;

namespace ChatAppAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = builder.Configuration.GetSection("JWT");

            // Add services to the container.
            builder.Services.AddInfrastructureServices();

            builder.Services.AddApplicationServices();

            builder.Services.AddAuthenticationServices(builder.Configuration);

            builder.Services.AddSignalR(hubOptions => {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
                hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });

            // Custom response
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GenericResponseFilter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Collect validation errors
                    List<string> errors = context.ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage)).ToList();

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

            // auto mapping
            builder.Services.AddAutoMapper(typeof(ViewModelToDtoProfile));
            builder.Services.AddAutoMapper(typeof(DtoToEntityProfile));

            // Add caching
            builder.Services.AddOutputCache();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(3);  // Token is valid for 3 hours
            });

            // max file size
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 5 * 1024 * 1024; // 5 MB
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
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOutputCache();

            app.UseStaticFiles();

            app.MapControllers();
            app.MapHub<ChatHub>("/Application/Hubs/chatHub");

            app.Run();
        }

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new[] { "Owner", "Admin", "User" };

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
