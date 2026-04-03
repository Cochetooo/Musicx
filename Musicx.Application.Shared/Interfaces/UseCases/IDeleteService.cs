namespace Musicx.Application.Shared.Interfaces.UseCases;

public interface IDeleteService<T> where T : class
{
    Task ExecuteAsync(long id);
    void Execute(long id);
}