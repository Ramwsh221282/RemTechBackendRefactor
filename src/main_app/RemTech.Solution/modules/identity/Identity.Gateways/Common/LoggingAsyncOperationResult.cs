using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Gateways.Common;

public sealed class LoggingAsyncOperationResult(
    string operationName,
    Serilog.ILogger logger,
    IAsyncOperationResult result) : IAsyncOperationResult
{
    public async Task<Result<Unit>> Process()
    {
        logger.Information("Processing operation: {name}", operationName);
        Result<Unit> res = await result.Process();
        logger.Information("Completed operation: {name}", operationName);
        if (res.IsFailure) logger.Error("{Error}", res.Error);
        return res;
    }
}