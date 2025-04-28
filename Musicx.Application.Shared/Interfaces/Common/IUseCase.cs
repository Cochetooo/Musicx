namespace Musicx.Application.Shared.Interfaces.Common;

/// <summary>
/// Base behavior of a use case.
/// </summary>
/// <typeparam name="TRequest">A class inheriting from <see cref="BaseRequest"/> that will serve as input values.</typeparam>
/// <typeparam name="TResponse">A class inheriting from <see cref="BaseResponse"/> that will serve as output values.</typeparam>
/// <since>0.6.1</since>
public interface IUseCase<in TRequest, TResponse>
    where TRequest : BaseRequest
    where TResponse : BaseResponse
{
    /// <summary>
    /// Execute asynchronously the use case.
    /// </summary>
    /// <param name="request">A request dto with all parameters used for this use case</param>
    /// <returns>A Dto as response.</returns>
    Task<TResponse> ExecuteAsync(TRequest request);
    
    /// <summary>
    /// Execute the use case.
    /// </summary>
    /// <param name="request">A request dto with all parameters used for this use case</param>
    /// <returns>A Dto as response.</returns>
    TResponse Execute(TRequest request);
}

/// <summary>
/// Base behavior for a request data object.
/// </summary>
/// <since>0.6.1</since>
public abstract record BaseRequest;

/// <summary>
/// Base behavior for a response data object.
/// </summary>
/// <since>0.6.1</since>
public abstract record BaseResponse;