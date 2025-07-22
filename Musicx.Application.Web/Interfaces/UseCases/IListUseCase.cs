namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IListUseCase<T> where T : class
{
    Task<List<T>> ExecuteAsync(int skip = 0, int take = 200, string? filter = null, string query = "");
    List<T> Execute(int skip = 0, int take = 200, string? filter = null, string query = "");
}