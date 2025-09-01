using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases;

public interface ICountUseCase<T> where T : BaseOutputModel
{
    Task<long> ExecuteAsync();
    long Execute();
}