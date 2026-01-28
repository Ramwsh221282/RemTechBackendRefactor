using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.SubscribeParser;

/// <summary>
/// Обработчик команды подписки на парсер.
/// </summary>
/// <param name="repository">Репозиторий подписанных парсеров.</param>
/// <param name="listener">Слушатель событий подписки на парсер.</param>
[TransactionalHandler]
public sealed class SubscribeParserHandler(
	ISubscribedParsersRepository repository,
	IOnParserSubscribedListener listener
) : ICommandHandler<SubscribeParserCommand, SubscribedParser>
{
	/// <summary>
	/// Выполняет команду подписки на парсер.
	/// </summary>
	/// <param name="command">Команда подписки на парсер.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения команды с новым подписанным парсером.</returns>
	public async Task<Result<SubscribedParser>> Execute(SubscribeParserCommand command, CancellationToken ct = default)
	{
		Result<SubscribedParser> result = await ProcessParserSubscription(command, ct);
		await NotifyListener(result, ct);
		return result;
	}

	private Task<Result<SubscribedParser>> ProcessParserSubscription(
		SubscribeParserCommand command,
		CancellationToken ct = default
	)
	{
		SubscribedParserId id = SubscribedParserId.Create(command.Id).Value;
		SubscribedParserIdentity identity = SubscribedParserIdentity.Create(command.Domain, command.Type);
		return SubscribedParser.CreateNew(id, identity, repository, ct: ct);
	}

	private async Task NotifyListener(Result<SubscribedParser> result, CancellationToken ct = default)
	{
		if (result.IsFailure)
			return;
		await listener.Handle(result.Value, ct: ct);
	}
}
