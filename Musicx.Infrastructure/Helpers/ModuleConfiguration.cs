using Microsoft.Extensions.Configuration;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;

namespace Musicx.Infrastructure.Helpers;

public class ModuleConfiguration(ILoggerFactory loggerFactory, IConfiguration configuration) : IModuleConfiguration
{
    private ILogger<ModuleConfiguration> Logger = loggerFactory.CreateLogger<ModuleConfiguration>();

    public T GetValue<T>(string key)
    {
        string? value = configuration[key];

        if (null == value)
        {
            Logger.Error($"❌ The value for key {key} was not found.");
            throw new InvalidOperationException();
        }
        
        return (T) Convert.ChangeType(value, typeof(T));
    }
}