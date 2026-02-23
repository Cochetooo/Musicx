using Musicx.Application.Api.Interfaces.Storage;

namespace Musicx.Presentation.Web.Storage;

public sealed class WebRootFileRoot(IWebHostEnvironment env, IHttpContextAccessor http) : IPublicFileRoot
{
    public string GetWebRootPath() => env.WebRootPath;
    
    public string GetContentRootPath() => env.ContentRootPath;

    public string GetBaseUrl()
    {
        var req = http.HttpContext!.Request;
        return $"{req.Scheme}://{req.Host}";
    }
}