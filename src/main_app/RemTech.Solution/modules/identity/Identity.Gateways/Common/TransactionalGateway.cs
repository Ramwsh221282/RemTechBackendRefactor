using System.Reflection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Common;

public sealed class TransactionalGateway<TRequest, TResponse>(
    ITransactionalOperation transactional,
    IGateway<TRequest, TResponse> origin)
    : IGateway<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    public async Task<Result<TResponse>> Execute(TRequest request)
    {
        CancellationToken cancellationToken = InspectForCancellationToken(request);
        AsyncOperation<TResponse> inner = new(async () => await origin.Execute(request));
        return await transactional.Execute(inner, cancellationToken);
    }

    private CancellationToken InspectForCancellationToken(TRequest request)
    {
        Type requestType = request.GetType();
        CancellationToken[] tokens = 
            InspectForCancellationTokenUsingFields(requestType, request)
            .Concat(InspectForCancellationTokenUsingProperties(requestType, requestType))
            .ToArray();
        return tokens.Length == 0 ? CancellationToken.None : tokens[0];
    }
    
    private CancellationToken[] InspectForCancellationTokenUsingProperties(
        Type objectType,
        object @object
        )
    {
        return @objectType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(f => f.GetValue(@object))
            .OfType<CancellationToken>()
            .ToArray();
    }
    
    private CancellationToken[] InspectForCancellationTokenUsingFields(
        Type objectType, 
        object @object)
    {
        return @objectType
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(f => f.GetValue(@object))
            .OfType<CancellationToken>()
            .ToArray();
    }
}