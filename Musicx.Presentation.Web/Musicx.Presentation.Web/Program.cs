using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Musicx.Application.Api.Interfaces.Storage;
using Musicx.Infrastructure;
using Musicx.Presentation.Web.Client;
using Musicx.Presentation.Web.Components;
using Musicx.Presentation.Web.Contexts;
using Musicx.Presentation.Web.Middlewares.Auth;
using Musicx.Presentation.Web.Storage;
using _Imports = Musicx.Presentation.Web.Client._Imports;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

// Add blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddLocalization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .WithOrigins("https://localhost:7287")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

// Add application infrastructure
builder.Services
    .AddMusicxInfrastructure(builder.Configuration)
    .AddMusicxApi(builder.Configuration);

// Add Api dependencies
builder.Services
    .AddSingleton<IPublicFileRoot, WebRootFileRoot>()
    .AddScoped<IUserContext, UserContext>();

// Add Swagger
builder.Services
    .AddEndpointsApiExplorer()
    .AddOpenApi();

// Add Front-end
builder.Services
    .AddFrontFramework();

// Add endpoints
builder.Services
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Middlewares

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("fr") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders = 
    [
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    ]
});

app.UseMiddleware<AuthenticationMiddleware>();

app.MapStaticAssets();
app.MapControllers();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

app.Run();