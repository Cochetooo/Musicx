using System.Text.Json;
using System.Text.Json.Serialization;

namespace Musicx.Infrastructure.Shared.Models.ExternalMusicData.Helpers;

public sealed class ObjectOrStringConverter<T> : JsonConverter<T> where T : new()
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return JsonSerializer.Deserialize<T>(ref reader, options)!;
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            reader.GetString();
            return new T();
        }

        throw new JsonException($"Unsupported token type: {reader.TokenType}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}