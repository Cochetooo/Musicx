using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Musicx.Infrastructure.API.Persistence.Converters;

public sealed class AlbumInfluenceNodeConverter : JsonConverter<AlbumInfluenceNode>
{
    public override AlbumInfluenceNode ReadJson(
        JsonReader reader,
        Type objectType,
        AlbumInfluenceNode? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        var node = new AlbumInfluenceNode
        {
            Genre = obj["genre"]!.ToObject<OutGenre>(serializer)!,
            Relation = obj["relation"]!.ToObject<OutAlbumInfluence>(serializer)!
        };

        return node;
    }

    public override void WriteJson(
        JsonWriter writer,
        AlbumInfluenceNode? value,
        JsonSerializer serializer)
        => throw new NotImplementedException();
}