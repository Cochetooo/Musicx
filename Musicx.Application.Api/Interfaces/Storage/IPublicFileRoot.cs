namespace Musicx.Application.Api.Interfaces.Storage;

public interface IPublicFileRoot
{
    string GetWebRootPath();
    string GetContentRootPath();
    string GetBaseUrl();
}