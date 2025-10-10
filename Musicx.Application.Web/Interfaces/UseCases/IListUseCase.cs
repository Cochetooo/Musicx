namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IListUseCase<T> where T : class
{
    Task<List<T>> ExecuteAsync(long skip = 0, long take = 200, 
        bool? filterExact = null, double? filterSimilitude = null, 
        string? filter = null, string? order = null, string query = "");
    List<T> Execute(long skip = 0, long take = 200, 
        bool? filterExact = null, double? filterSimilitude = null, 
        string? filter = null, string? order = null, string query = "");
}