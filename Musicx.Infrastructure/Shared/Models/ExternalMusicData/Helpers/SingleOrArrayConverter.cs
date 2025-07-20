using System.Text.Json;
using System.Text.Json.Serialization;

namespace Musicx.Infrastructure.Shared.Models.ExternalMusicData.Helpers;

public sealed class SingleOrArrayConverter<T> : JsonConverter<List<T>>
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<T>();

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var element = JsonSerializer.Deserialize<T>(ref reader, options);
            result.Add(element!);
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            result = JsonSerializer.Deserialize<List<T>>(ref reader, options)!;
        }
        else if (reader.TokenType == JsonTokenType.Null)
        {
            return new List<T>();
        }
        else
        {
            throw new JsonException();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        if (value.Count == 1)
        {
            JsonSerializer.Serialize(writer, value[0], options);
        }
        else
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}