namespace Musicx.Application.Web.Interfaces.UseCases;

public interface ISaveService<in T> where T : class
{
    Task<HttpResponseMessage> ExecuteAsync(T entity);
    HttpResponseMessage Execute(T entity);
}