namespace RemTech.SharedKernel.Core.Handlers;

/// <summary>
/// Интерфейс обработчика запросов.
/// </summary>
/// <typeparam name="TQuery">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип ответа.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
	where TQuery : IQuery
{
	/// <summary>
	/// Обрабатывает запрос и возвращает ответ.
	/// </summary>
	/// <param name="query">Запрос для обработки.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Ответ на запрос.</returns>
	Task<TResponse> Handle(TQuery query, CancellationToken ct = default);
}
