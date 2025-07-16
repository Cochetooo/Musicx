namespace Musicx.Application.Web.Interfaces.UseCases;

public interface ISaveUseCase<in T> where T : class
{
    Task ExecuteAsync(T entity);
    void Execute(T entity);
}