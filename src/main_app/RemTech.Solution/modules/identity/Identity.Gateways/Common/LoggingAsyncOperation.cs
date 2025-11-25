using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.Common;

public sealed class LoggingAsyncOperation<T>(
    string operationName,
    Serilog.ILogger logger,
    IAsyncOperation<T> result) : IAsyncOperation<T>
{
    public async Task<Result<T>> Process()
    {
        logger.Information("Processing operation: {name}", operationName);
        Result<T> res = await result.Process();
        logger.Information("Completed operation: {name}", operationName);
        if (res.IsFailure) logger.Error("{Error}", res.Error);
        return res;
    }
}