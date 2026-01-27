using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers.Decorators.CacheInvalidate;

/// <summary>
/// Обработчик команд с декоратором для инвалидирования кэша.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
/// <param name="invalidators">Коллекция инвалидаторов кэша.</param>
/// <param name="inner">Внутренний обработчик команд.</param>
public sealed class CacheInvalidatingHandler<TCommand, TResult>(
	IEnumerable<ICacheInvalidator<TCommand, TResult>> invalidators,
	ICommandHandler<TCommand, TResult> inner
) : ICacheInvalidatingHandler<TCommand, TResult>
	where TCommand : ICommand
{
	private IEnumerable<ICacheInvalidator<TCommand, TResult>> Invalidators { get; } = invalidators;
	private ICommandHandler<TCommand, TResult> Inner { get; } = inner;

	/// <summary>
	/// Выполняет команду и инвалидирует кэш при успешном выполнении.
	/// </summary>
	/// <param name="command">Команда для выполнения.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения команды.</returns>
	public async Task<Result<TResult>> Execute(TCommand command, CancellationToken ct = default)
	{
		Result<TResult> result = await Inner.Execute(command, ct);
		if (result.IsFailure)
		{
			return result.Error;
		}

		foreach (ICacheInvalidator<TCommand, TResult> invalidator in Invalidators)
		{
			await invalidator.InvalidateCache(command, result.Value, ct);
		}

		return result;
	}
}
