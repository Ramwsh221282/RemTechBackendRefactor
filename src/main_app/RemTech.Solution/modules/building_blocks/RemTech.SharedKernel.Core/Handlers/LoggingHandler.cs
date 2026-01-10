using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public sealed class LoggingHandler<TCommand, TResult>(Serilog.ILogger logger, ICommandHandler<TCommand, TResult> handler) 
    : IValidatingCommandHandler<TCommand, TResult> where TCommand : ICommand
{
    private ICommandHandler<TCommand, TResult> Handler { get; } = handler;
    private Serilog.ILogger Logger { get; } = logger.ForContext<TCommand>();
    private string CommandName { get; } = typeof(TCommand).Name;
    
    public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
    {
        Logger.Information("Executing command: {Command}. Payload: {Payload}", CommandName, command.ToString());
        Result<TResult> result = await Handler.Execute(command);
        Logger.Information("Command executed: {Command}", CommandName);
        LogIfErrorResult(result);
        return result;
    }

    private void LogIfErrorResult(Result<TResult> result)
    {
        if (result.IsFailure)
            Logger.Error("Command failed: {Command}. Error: {Error}", CommandName, result.Error.Message);
    }
}