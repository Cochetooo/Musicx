namespace Musicx.Application.Web.Interfaces.UseCases;

public interface ISaveUseCase<in T> where T : class
{
    Task<HttpResponseMessage> ExecuteAsync(T entity);
    HttpResponseMessage Execute(T entity);
}