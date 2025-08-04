using Musicx.Infrastructure;
using Musicx.Presentation.Web.Client;
using Musicx.Presentation.Web.Components;
using _Imports = Musicx.Presentation.Web.Client._Imports;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

// Add blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add application infrastructure
builder.Services
    .AddMusicxInfrastructure(builder.Configuration)
    .AddMusicxApi(builder.Configuration);

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

app.MapStaticAssets();
app.MapControllers();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly);

app.Run();