namespace Musicx.Application.Api.Interfaces.Storage;

public interface IAvatarStorage
{
    Task<string> SaveAsync(
        long userId,
        Stream imageStream,
        string contentType,
        CancellationToken token = default
    );
    
    Task DeleteAsync(
        long userId, 
        CancellationToken token = default
    );
}