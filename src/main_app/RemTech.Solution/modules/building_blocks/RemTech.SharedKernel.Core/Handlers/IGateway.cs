using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public interface IGateway<in TRequest, TResponse>
where TRequest : IRequest 
where TResponse : IResponse
{
    Task<Result<TResponse>> Execute(TRequest request);
}