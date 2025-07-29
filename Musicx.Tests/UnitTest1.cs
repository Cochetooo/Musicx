using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Musicx.Application.Desktop;
using Musicx.Application.Desktop.Interfaces.UseCases.LocalLibrary;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Infrastructure;
using Musicx.Infrastructure.API.Persistence.Mappers;

namespace Musicx.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        
    }
    
    private static readonly JsonSerializerOptions GenreMapperJsonOptions = new()
    {
        PropertyNamingPolicy = new DbToOutModelPolicy("genre"),
    };

    [Test]
    public void JsonDeserialization_Test()
    {
        const string jsonString = """
                       [{"genre_id":1,"genre_created_at":"2025-07-25T14:07:16.198144","genre_updated_at":"2025-07-25T14:07:16.198347","genre_name":"Progressive Metal","genre_description":null,"genre_color":"#F59025"}]
                       """;
        
        var result = DebugDeserialization<GenreTest[]>(jsonString, GenreMapperJsonOptions);
        foreach (var genre in result)
        {
            Console.WriteLine("result: " + genre.Name + " | " + genre.Id);
        }
    }
    
    public static T? DebugDeserialization<T>(string json, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.Parse(json);

        Console.WriteLine("---- START DEBUG ----");
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            foreach (var prop in element.EnumerateObject())
            {
                var converted = options.PropertyNamingPolicy?.ConvertName(prop.Name);
                Console.WriteLine($"JSON: {prop.Name} -> Policy: {converted}");
            }
        }
        Console.WriteLine("---- END DEBUG ----");

        try
        {
            Console.WriteLine(json);
            var result = JsonSerializer.Deserialize<T>(json, options);
            Console.WriteLine("Désérialisation OK.");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXCEPTION !");
            Console.WriteLine(ex);
            throw;
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