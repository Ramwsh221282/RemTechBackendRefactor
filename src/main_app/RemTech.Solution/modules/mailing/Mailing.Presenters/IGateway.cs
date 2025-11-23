using RemTech.Functional.Extensions;

namespace Mailing.Presenters;

public interface IResponse;

public interface IRequest;

public interface IGateway<in TRequest, TResponse>
where TRequest : IRequest 
where TResponse : IResponse
{
    Task<Result<TResponse>> Execute(TRequest request);
}