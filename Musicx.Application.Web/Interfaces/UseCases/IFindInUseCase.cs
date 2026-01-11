using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IFindInUseCase<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    Task<List<TOut>> ExecuteAsync(
        IEnumerable<long> ids, 
        IJoinSpecification<TIn>? joins = null, 
        OrderSpecification<TIn>? order = null
    );
    
    List<TOut> Execute(
        IEnumerable<long> ids,
        IJoinSpecification<TIn>? joins = null, 
        OrderSpecification<TIn>? order = null
    );
}