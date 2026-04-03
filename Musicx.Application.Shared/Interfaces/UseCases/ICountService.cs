using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.UseCases;

public interface ICountService<T> where T : BaseOutputModel
{
    Task<long> ExecuteAsync();
    long Execute();
}