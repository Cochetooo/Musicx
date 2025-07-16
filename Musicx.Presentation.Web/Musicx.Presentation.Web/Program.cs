using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Musicx.Infrastructure;
using Musicx.Presentation.Web.Client;
using Musicx.Presentation.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add blazor services
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add application infrastructure
builder.Services
    .AddMusicxInfrastructure()
    .AddMusicxApi(builder.Configuration);

// Add Swagger
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Musicx.Presentation.Web.Client._Imports).Assembly);

app.MapControllers();

app.Run();