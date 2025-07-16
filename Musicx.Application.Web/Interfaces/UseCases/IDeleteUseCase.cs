namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IDeleteUseCase<T> where T : class
{
    Task ExecuteAsync(int id);
    void Execute(int id);
}