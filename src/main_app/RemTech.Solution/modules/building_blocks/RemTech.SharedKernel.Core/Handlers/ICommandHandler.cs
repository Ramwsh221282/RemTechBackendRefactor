using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

/// <summary>
/// Интерфейс обработчика команд.
/// </summary>
/// <typeparam name="TCommand">Команда.</typeparam>
/// <typeparam name="TResult">Результат выполнения команды.</typeparam>
public interface ICommandHandler<in TCommand, TResult>
	where TCommand : ICommand
{
	/// <summary>
	/// Выполняет команду и возвращает результат.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды.</returns>
	Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default);
}
