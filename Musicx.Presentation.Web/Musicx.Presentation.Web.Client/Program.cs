using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Musicx.Infrastructure;
using Musicx.Infrastructure.Shared.Providers.ExternalMusicData;
using Musicx.Presentation.Web.Client;
using Musicx.Presentation.Web.Client.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddFrontFramework();

builder.Services
    .AddMusicxWeb();

builder.Services.AddScoped(_ => new HttpClient
{
  BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

var app = builder.Build();

var jsRuntime = app.Services.GetRequiredService<IJSRuntime>();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
loggerFactory.AddProvider(new StyledJsConsoleLoggerProvider(jsRuntime));

var loggerProvider = app.Services.GetRequiredService<ILoggerProvider>();
var logger = loggerProvider.CreateLogger(nameof(Program));

logger.LogInformation("🏳️ Started new Client Side Session.");

await app.RunAsync();