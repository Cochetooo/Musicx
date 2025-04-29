namespace Musicx.Application.Api.Interfaces.UseCases;

public interface IDeleteUseCase<T> where T : class
{
    Task ExecuteAsync(int id);
    void Execute(int id);
}