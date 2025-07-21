namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IGetUseCase<T> where T : class
{
    Task<T?> ExecuteAsync(long id, string query = "");
    T? Execute(long id, string query = "");
}