using System.Reflection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace RemTech.SharedKernel.Infrastructure.NpgSql;

public sealed class TransactionalGateway<TRequest, TResponse>(
    NpgSqlSession session,
    IGateway<TRequest, TResponse> origin)
    : IGateway<TRequest, TResponse>
    where TRequest : IRequest
    where TResponse : IResponse
{
    public async Task<Result<TResponse>> Execute(TRequest request)
    {
        CancellationToken cancellationToken = InspectForCancellationToken(request);
        await session.GetTransaction(cancellationToken);
        Result<TResponse> result = await origin.Execute(request);
        if (result.IsFailure) return result.Error;
        return !await session.Commited(cancellationToken) ? Error.Application("Ошибка транзакции.") : result;
    }

    private CancellationToken InspectForCancellationToken(TRequest request)
    {
        Type requestType = request.GetType();
        CancellationToken[] tokens = 
            InspectForCancellationTokenUsingFields(requestType, request)
            .Concat(InspectForCancellationTokenUsingProperties(requestType, request))
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