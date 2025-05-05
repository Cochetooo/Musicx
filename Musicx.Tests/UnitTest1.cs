using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Infrastructure;

namespace Musicx.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task ImportLocalSongsAsync()
    {
        var services = new ServiceCollection();

        services
            .AddMusicxDesktopApp()
            .AddMusicxInfrastructure()
            .AddMusicxDesktop();
        
        var provider = services.BuildServiceProvider();
        
        var useCase = provider.GetRequiredService<IImportLocalSongsUseCase>();

        var response = await useCase.ExecuteAsync(new ImportLocalSongsRequest(
            [@"Z:\Sort\Music\SoulSeek Downloads\complete\Al-Kramer"],
            [".mp3"],
            false,
            new ProgressListener()));

        Assert.Multiple(() =>
        {
            Assert.That(response.TotalFileCount, Is.EqualTo(21));
        });
    }

    class ProgressListener : IProgressListener
    {
        public object? Parameters { get; set; }
        public int Progress { get; set; }
        public int Total { get; set; }

        public void UpdateProgress(int progress, int total, object? parameters)
        {
            Parameters = parameters;
            Total = total;
            Progress = progress;
        }
    }
}