using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Musicx.Infrastructure;
using Musicx.Presentation.Web.Client;
using Musicx.Presentation.Web.Client.Handlers;
using Musicx.Presentation.Web.Client.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddFrontFramework();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services
    .AddMusicxWeb();

builder.Services.AddTransient<BrowserCredentialsHandler>();

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
})
.AddHttpMessageHandler<BrowserCredentialsHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

var app = builder.Build();

var jsRuntime = app.Services.GetRequiredService<IJSRuntime>();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
loggerFactory.AddProvider(new StyledJsConsoleLoggerProvider(jsRuntime));

var logger = loggerFactory.CreateLogger(nameof(Program));

logger.LogInformation("🏳️ Started new Client Side Session.");

await app.RunAsync();