using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Musicx.Infrastructure;
using Musicx.Infrastructure.Shared.Providers.ExternalMusicData;
using Musicx.Presentation.Web.Client;
using Musicx.Presentation.Web.Client.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

const string json = """
                    {
                      "Apis": {
                        "LastFm": {
                          "BaseUrl": "https://ws.audioscrobbler.com/2.0/",
                          "ApiKey": "f17c4b2c645d912c0f222eaa77f58751"
                        },
                        "Itunes": {
                          "BaseUrl": "https://itunes.apple.com/"
                        }
                      }
                    }
                    """;

var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
builder.Configuration.AddJsonStream(stream);

builder.Services.AddFrontFramework();

builder.Services
    .AddMusicxInfrastructure()
    .AddMusicxWeb();

builder.Services.AddHttpClient<LastFmApiProvider>(client =>
{
  client.BaseAddress = new Uri(builder.Configuration["Apis:LastFm:BaseUrl"]!);
});

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
logger.LogInformation("🌍 LastFm URL: " + builder.Configuration["Apis:LastFm:BaseUrl"]);

await app.RunAsync();