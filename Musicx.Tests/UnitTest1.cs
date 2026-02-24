using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Infrastructure;
using Musicx.Infrastructure.API.Persistence.Mappers;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Musicx.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        
    }
    
    private static readonly JsonSerializerSettings GenreMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenre>("genre")
        }
    };

    [Test]
    public void JsonDeserialization_Test()
    {
        try
        {
            const string jsonString = """
                                      [{"genre_id":1,"genre_created_at":"2025-07-25T14:07:16.198144","genre_updated_at":"2025-07-25T14:07:16.198347","genre_name":"Progressive Metal","genre_description":null,"genre_color":"#F59025"}]
                                      """;

            var result = JsonConvert.DeserializeObject<OutGenre[]>(jsonString, GenreMapperJsonOptions);
            foreach (var genre in result)
            {
                Console.WriteLine("result: " + genre.CanonicalName + " | " + genre.CreatedAt);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    class GenreTest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}