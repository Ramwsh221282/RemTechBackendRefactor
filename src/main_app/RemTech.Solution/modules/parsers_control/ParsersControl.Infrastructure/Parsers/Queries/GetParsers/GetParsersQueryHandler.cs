using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

/// <summary>
///  Обработчик запроса на получение всех парсеров.
/// </summary>
/// <param name="repository">Экземпляр репозитория подписанных парсеров.</param>
public sealed class GetParsersQueryHandler(ISubscribedParsersCollectionRepository repository)
	: IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>>
{
	private ISubscribedParsersCollectionRepository Repository { get; } = repository;

	/// <summary>
	/// Обрабатывает запрос на получение всех парсеров.
	/// </summary>
	/// <param name="query">Запрос на получение всех парсеров.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с информацией о парсерах.</returns>
	public async Task<IEnumerable<ParserResponse>> Handle(GetParsersQuery query, CancellationToken ct = default)
	{
		SubscribedParsersCollectionQuery emptyQuery = new();
		SubscribedParsersCollection parsers = await Repository.Read(emptyQuery, ct);
		return parsers.IsEmpty() ? [] : parsers.Read().Select(ParserResponse.Create);
	}
}
