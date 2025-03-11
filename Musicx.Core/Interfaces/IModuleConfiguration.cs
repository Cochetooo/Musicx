namespace Musicx.Core.Interfaces;

public interface IModuleConfiguration
{
    T GetValue<T>(string key);
}