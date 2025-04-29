namespace Musicx.Application.Api.Interfaces.UseCases;

public interface ISaveUseCase<T> where T : class
{
    Task ExecuteAsync(T entity);
    void Execute(T entity);
}