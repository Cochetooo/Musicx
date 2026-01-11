using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IDeleteUseCase<T> where T : class
{
    Task ExecuteAsync(long id);
    void Execute(long id);
}