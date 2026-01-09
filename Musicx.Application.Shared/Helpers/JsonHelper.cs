using System.Text.Json;

namespace Musicx.Application.Shared.Helpers;

public static class JsonHelper
{
    public static readonly JsonSerializerOptions OptionsDefault = new()
    {
        PropertyNameCaseInsensitive = true
    };
}