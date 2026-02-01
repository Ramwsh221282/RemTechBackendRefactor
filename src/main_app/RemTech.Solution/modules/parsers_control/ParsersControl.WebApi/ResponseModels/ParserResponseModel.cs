using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.WebApi.ResponseModels;

/// <summary>
/// Модель ответа для информации о парсере.
/// </summary>
public sealed class ParserResponseModel
{
	/// <summary>
	/// Идентификатор парсера.
	/// </summary>
	public required Guid Id { get; init; }

	/// <summary>
	///     Тип парсера.
	/// </summary>
	public required string Type { get; init; }

	/// <summary>
	///    Домен парсера.
	/// </summary>
	public required string Domain { get; init; }

	/// <summary>
	///   Состояние парсера.
	/// </summary>
	public required string State { get; init; }

	/// <summary>
	///     Время начала парсера.
	/// </summary>
	public required DateTime? StartedAt { get; init; }

	/// <summary>
	/// Количество обработанных элементов.
	/// </summary>
	public required int Processed { get; init; }

	/// <summary>
	/// Затраченное время.
	/// </summary>
	public required int ElapsedHours { get; init; }

	/// <summary>
	/// Затраченное время.
	/// </summary>
	public required int ElapsedMinutes { get; init; }

	/// <summary>
	/// Затраченное время.
	/// </summary>
	public required int ElapsedSeconds { get; init; }

	/// <summary>
	/// Количество дней ожидания до следующего запуска.
	/// </summary>
	public required int? WaitDays { get; init; }

	/// <summary>
	/// Время следующего запуска.
	/// </summary>
	public required DateTime? NextRun { get; init; }

	/// <summary>
	///     Время окончания парсера.
	/// </summary>
	public required DateTime? FinishedAt { get; init; }

	/// <summary>
	///   Преобразование коллекции парсеров в модели ответа.
	/// </summary>
	/// <param name="parsers">Коллекция парсеров для преобразования.</param>
	/// <returns>Коллекция моделей ответа парсеров.</returns>
	public static IEnumerable<ParserResponseModel> ConvertFrom(IEnumerable<SubscribedParser> parsers)
	{
		return parsers.Select(ConvertFrom).ToArray();
	}

	/// <summary>
	/// Преобразование парсера в модель ответа.
	/// </summary>
	/// <param name="parser">Парсер для преобразования.</param>
	/// <returns>Модель ответа парсера.</returns>
	public static ParserResponseModel ConvertFrom(SubscribedParser parser)
	{
		return new()
		{
			Id = parser.Id.Value,
			Type = parser.Identity.ServiceType,
			Domain = parser.Identity.DomainName,
			State = parser.State.Value,
			StartedAt = parser.Schedule.StartedAt,
			Processed = parser.Statistics.ParsedCount.Value,
			ElapsedHours = parser.Statistics.WorkTime.Hours,
			ElapsedMinutes = parser.Statistics.WorkTime.Minutes,
			ElapsedSeconds = parser.Statistics.WorkTime.Seconds,
			WaitDays = parser.Schedule.WaitDays,
			NextRun = parser.Schedule.NextRun,
			FinishedAt = parser.Schedule.FinishedAt,
		};
	}
}
