using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParser;

/// <summary>
///   Обработчик запроса на получение парсера по его идентификатору.
/// </summary>
/// <param name="repository">Экземпляр репозитория подписанных парсеров.</param>
public sealed class GetParserQueryHandler(ISubscribedParsersRepository repository)
	: IQueryHandler<GetParserQuery, ParserResponse?>
{
	private ISubscribedParsersRepository Repository { get; } = repository;

	/// <summary>
	///  Обрабатывает запрос на получение парсера по его идентификатору.
	/// </summary>
	/// <param name="query">Запрос на получение парсера.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат выполнения запроса - парсер или null, если парсер не найден.</returns>
	public async Task<ParserResponse?> Handle(GetParserQuery query, CancellationToken ct = default)
	{
		SubscribedParserQuery spec = new SubscribedParserQuery().WithId(query.Id);
		Result<SubscribedParser> result = await Repository.Read(spec, ct);
		return result.IsFailure ? null : ParserResponse.Create(result.Value);
	}
}
