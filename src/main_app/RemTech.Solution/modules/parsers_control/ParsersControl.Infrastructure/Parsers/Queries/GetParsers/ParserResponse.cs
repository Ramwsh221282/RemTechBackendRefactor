using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

/// <summary>
/// Ответ с информацией о парсере.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
/// <param name="Domain">Домен парсера.</param>
/// <param name="ServiceType">Тип сервиса парсера.</param>
/// <param name="FinishedAt">Время завершения парсера.</param>
/// <param name="StartedAt">Время запуска парсера.</param>
/// <param name="NextRun">Время следующего запуска парсера.</param>
/// <param name="WaitDays">Количество дней ожидания парсера.</param>
/// <param name="State">Состояние парсера.</param>
/// <param name="ParsedCount">Количество обработанных элементов парсером.</param>
/// <param name="ElapsedHours">Количество прошедших часов работы парсера.</param>
/// <param name="ElapsedSeconds">Количество прошедших секунд работы парсера.</param>
/// <param name="ElapsedMinutes">Количество прошедших минут работы парсера.</param>
/// <param name="Links">Коллекция ссылок на парсер.</param>
public sealed record ParserResponse(
	Guid Id,
	string Domain,
	string ServiceType,
	DateTime? FinishedAt,
	DateTime? StartedAt,
	DateTime? NextRun,
	int? WaitDays,
	string State,
	int ParsedCount,
	int ElapsedHours,
	int ElapsedSeconds,
	int ElapsedMinutes,
	IEnumerable<ParserLinkResponse> Links
)
{
	/// <summary>
	///   Создаёт ответ с информацией о парсере из модели подписанного парсера.
	/// </summary>
	/// <param name="parser">Модель подписанного парсера.</param>
	/// <returns>Ответ с информацией о парсере.</returns>
	public static ParserResponse Create(SubscribedParser parser)
	{
		return new(
			parser.Id.Value,
			parser.Identity.DomainName,
			parser.Identity.ServiceType,
			parser.Schedule.FinishedAt,
			parser.Schedule.StartedAt,
			parser.Schedule.NextRun,
			parser.Schedule.WaitDays,
			parser.State.Value,
			parser.Statistics.ParsedCount.Value,
			parser.Statistics.WorkTime.Hours,
			parser.Statistics.WorkTime.Minutes,
			parser.Statistics.WorkTime.Seconds,
			parser.Links.Select(ParserLinkResponse.Create)
		);
	}
}
