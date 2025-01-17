using ClientSidee;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Existing HttpClient registration
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44385/") });

// Register AuthService
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();