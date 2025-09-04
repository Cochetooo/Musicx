using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Web.Interfaces.UseCases;

public interface ISaveAllUseCase<in T> where T : BaseInputModel
{
    Task ExecuteAsync(IEnumerable<T> entities);
    void Execute(IEnumerable<T> entities);
}