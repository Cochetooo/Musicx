using System.Reflection;
using Musicx.Contracts.Dto.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public sealed class JsonToOutModelConverter<T>(
    string prefixToTrim) : JsonConverter where T : new()
{
    public override bool CanWrite => false; // Désérialisation uniquement

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(T);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var target = new T();

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var jProp in jsonObject.Properties())
        {
            if (jProp.Value.Type == JTokenType.Null)
            {
                continue;
            }
            
            var name = jProp.Name;

            // 1. Supprimer le préfixe
            if (name.StartsWith(prefixToTrim + "_"))
            {
                name = name.Substring(prefixToTrim.Length + 1);
            }

            // 2. snake_case → PascalCase
            var pascalCaseName = string.Concat(
                name.Split('_', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1)));

            var prop = props.FirstOrDefault(p => p.Name == pascalCaseName);
            if (prop != null && prop.CanWrite)
            {
                if (prop.PropertyType == typeof(string) && jProp.Value.Type == JTokenType.Object)
                {
                    prop.SetValue(target, jProp.Value.ToString(Formatting.None));
                }
                else
                {
                    var value = jProp.Value.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(target, value);
                }
            }
        }

        return target;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Ce converter est pour désérialisation uniquement.");
    }
}