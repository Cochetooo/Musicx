using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Web.Interfaces.UseCases;

public interface IFindAllService<TIn, TOut> 
    where TIn : BaseInputModel
    where TOut : BaseOutputModel
{
    Task<List<TOut>> ExecuteAsync(
        bool? filterExact = null, 
        double? filterSimilitude = null, 
        string? filter = null, 
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null
    );
    
    List<TOut> Execute(
        bool? filterExact = null, 
        double? filterSimilitude = null, 
        string? filter = null, 
        IJoinSpecification<TIn>? joins = null,
        OrderSpecification<TIn>? order = null,
        PagingOptions? pagingOptions = null
    );
}