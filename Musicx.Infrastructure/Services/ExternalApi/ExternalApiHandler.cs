using log4net;
using Musicx.Core.Models;
using Newtonsoft.Json.Linq;

namespace Musicx.Infrastructure.Services.ExternalApi;

public abstract class ExternalApiHandler(string apiUrl)
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(ExternalApiHandler));

    protected readonly string ApiUrl = apiUrl;

    public abstract Task<Song> FetchSong(Song song);
    public abstract Task<Album> FetchAlbum(Album album);
    public abstract Task<Artist> FetchArtist(Artist artist);

    protected abstract string MakeUrl(string method, Dictionary<string, string> parameters);

    protected async Task<JObject> SendRequestAsync(string requestUrl)
    {
        using var httpClient = new HttpClient();
        
        try
        {
            var response = await httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JObject.Parse(content);
            }
                   
            Logger.Error($"Api Call Error: {requestUrl}");
            return new JObject();
        }
        catch (HttpRequestException httpRequestException)
        {
            Logger.Error($"Request Error: {httpRequestException.Message}");
            return new JObject();
        }
        catch (Exception ex)
        {
            Logger.Error($"Unknown Error: {ex.Message}");
            return new JObject();
        }
    }
}