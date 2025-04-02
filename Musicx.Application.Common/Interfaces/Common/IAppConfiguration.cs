namespace Musicx.Application.Common.Interfaces.Common;

public interface IAppConfiguration
{
    T GetValue<T>(string key);
    void SetValue(string key, object value);
}