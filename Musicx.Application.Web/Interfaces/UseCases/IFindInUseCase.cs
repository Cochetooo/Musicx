namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IFindInUseCase<T> where T : class
{
    Task<List<T>> ExecuteAsync(IEnumerable<long> ids, string query = "");
    List<T> Execute(IEnumerable<long> ids, string query = "");
}