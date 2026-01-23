using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

public sealed class LoggingHandler<TCommand, TResult>(
	Serilog.ILogger logger,
	ICommandHandler<TCommand, TResult> handler
) : IValidatingCommandHandler<TCommand, TResult>
	where TCommand : ICommand
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<TCommand>();
	private string CommandName { get; } = typeof(TCommand).Name;

	public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
	{
		LogCommandEntered(command);
		Result<TResult> result = await handler.Execute(command, ct);
		LogCommandExited(result);
		return result;
	}

	private void LogCommandExited(Result<TResult> result)
	{
		if (result.IsFailure)
		{
			string error = result.Error.Message;
			Logger.Error("Command {Command} exited with error: {Error}", CommandName, error);
		}
		else
		{
			Logger.Information("Command {Command} executed successfully.", CommandName);
		}
	}

	private void LogCommandEntered(TCommand command)
	{
		Logger.Information("Executing command: {Command}.", CommandName);
		Logger.Information("Command payload: {Payload}", command.ToString());
	}
}
