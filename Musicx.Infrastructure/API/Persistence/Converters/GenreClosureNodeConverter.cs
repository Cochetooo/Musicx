using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Musicx.Infrastructure.API.Persistence.Converters;

public sealed class GenreClosureNodeConverter : JsonConverter<GenreClosureNode>
{
    public override GenreClosureNode ReadJson(
        JsonReader reader,
        Type objectType,
        GenreClosureNode? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        var node = new GenreClosureNode
        {
            Depth = obj["depth"]!.Value<int>(),
            Relation = obj["relation"]!.ToObject<OutGenre>(serializer)!
        };

        return node;
    }

    public override void WriteJson(
        JsonWriter writer,
        GenreClosureNode? value,
        JsonSerializer serializer)
        => throw new NotImplementedException();
}