using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Musicx.Infrastructure.API.Persistence.Converters;

public sealed class AlbumGenreNodeConverter : JsonConverter<AlbumGenreNode>
{
    public override AlbumGenreNode ReadJson(
        JsonReader reader,
        Type objectType,
        AlbumGenreNode? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        var node = new AlbumGenreNode
        {
            Genre = obj["genre"]!.ToObject<OutGenre>(serializer)!,
            Relation = obj["relation"]!.ToObject<OutAlbumGenre>(serializer)!
        };

        return node;
    }

    public override void WriteJson(
        JsonWriter writer,
        AlbumGenreNode? value,
        JsonSerializer serializer)
        => throw new NotImplementedException();
}
