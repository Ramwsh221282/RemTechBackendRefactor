using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Serilog;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.Logging;

/// <summary>
/// Декоратор логирующего обработчика команд.
/// </summary>
/// <typeparam name="TCommand">Команда.</typeparam>
/// <typeparam name="TResult">Результат.</typeparam>
/// <param name="logger">Логгер.</param>
/// <param name="handler">Обработчик команды.</param>
public sealed class LoggingCommandHandler<TCommand, TResult>(ILogger logger, ICommandHandler<TCommand, TResult> handler)
	: ILoggingCommandHandler<TCommand, TResult>
	where TCommand : ICommand
{
	private ILogger Logger { get; } = logger.ForContext<TCommand>();
	private string CommandName { get; } = typeof(TCommand).Name;

	/// <summary>
	/// Выполняет команду с логированием входа и выхода.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды.</returns>
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
