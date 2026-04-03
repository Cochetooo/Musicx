using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.UseCases;

public interface IFindOneByIdService<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    Task<TOut?> ExecuteAsync(long id, IJoinSpecification<TIn>? joins = null);
    TOut? Execute(long id, IJoinSpecification<TIn>? joins = null);
}