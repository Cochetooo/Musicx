using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Shared.Interfaces.UseCases;

public interface ISaveAllService<in T> where T : BaseInputModel
{
    Task<HttpResponseMessage> ExecuteAsync(IEnumerable<T> entities);
    HttpResponseMessage Execute(IEnumerable<T> entities);
}