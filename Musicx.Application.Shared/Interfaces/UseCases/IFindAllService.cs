using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Application.Shared.Interfaces.UseCases;

public interface IFindAllService<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    Task<OutGenericList<TOut>> ExecuteAsync(
        bool? filterExact = null, 
        double? filterSimilitude = null, 
        string? filter = null, 
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null
    );
    
    OutGenericList<TOut> Execute(
        bool? filterExact = null, 
        double? filterSimilitude = null, 
        string? filter = null, 
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null
    );
}