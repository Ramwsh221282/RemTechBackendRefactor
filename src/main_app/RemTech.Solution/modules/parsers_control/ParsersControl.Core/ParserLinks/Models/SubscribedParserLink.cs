using ParsersControl.Core.Common;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinks.Models;

/// <summary>
/// Ссылка на подписанный парсер.
/// </summary>
public sealed class SubscribedParserLink
{
	/// <summary>
	/// Создаёт новую ссылку на парсер.
	/// </summary>
	/// <param name="parserId">Идентификатор парсера.</param>
	/// <param name="id">Идентификатор ссылки.</param>
	/// <param name="urlInfo">Информация о ссылке.</param>
	/// <param name="statistics">Статистика парсинга.</param>
	/// <param name="active">Признак активности.</param>
	public SubscribedParserLink(
		SubscribedParserId parserId,
		SubscribedParserLinkId id,
		SubscribedParserLinkUrlInfo urlInfo,
		ParsingStatistics statistics,
		bool active
	)
	{
		ParserId = parserId;
		Id = id;
		UrlInfo = urlInfo;
		Statistics = statistics;
		Active = active;
	}

	/// <summary>
	/// Идентификатор парсера.
	/// </summary>
	public SubscribedParserId ParserId { get; }

	/// <summary>
	/// Идентификатор ссылки.
	/// </summary>
	public SubscribedParserLinkId Id { get; }

	/// <summary>
	/// Информация о ссылке.
	/// </summary>
	public SubscribedParserLinkUrlInfo UrlInfo { get; private set; }

	/// <summary>
	/// Статистика парсинга.
	/// </summary>
	public ParsingStatistics Statistics { get; private set; }

	/// <summary>
	/// Признак активности ссылки.
	/// </summary>
	public bool Active { get; private set; }

	/// <summary>
	/// Создаёт новую ссылку на парсер.
	/// </summary>
	/// <param name="parser">Экземпляр парсера.</param>
	/// <param name="urlInfo">Информация о ссылке.</param>
	/// <returns>Ссылка на парсер.</returns>
	public static SubscribedParserLink New(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo)
	{
		return new SubscribedParserLink(
			parser.Id,
			SubscribedParserLinkId.New(),
			urlInfo,
			ParsingStatistics.New(),
			false
		);
	}

	/// <summary>
	/// Создаёт ссылку на парсер с заданными параметрами.
	/// </summary>
	/// <param name="parserId">Идентификатор парсера.</param>
	/// <param name="id">Идентификатор ссылки.</param>
	/// <param name="urlInfo">Информация о ссылке.</param>
	/// <param name="statistics">Статистика парсинга.</param>
	/// <param name="active">Признак активности.</param>
	/// <returns>Ссылка на парсер.</returns>
	public static SubscribedParserLink Create(
		SubscribedParserId parserId,
		SubscribedParserLinkId id,
		SubscribedParserLinkUrlInfo urlInfo,
		ParsingStatistics statistics,
		bool active
	)
	{
		return new(parserId, id, urlInfo, statistics, active);
	}

	/// <summary>
	/// Создаёт копию ссылки на парсер.
	/// </summary>
	/// <param name="link">Ссылка для копирования.</param>
	/// <returns>Копия ссылки.</returns>
	public static SubscribedParserLink CreateCopy(SubscribedParserLink link)
	{
		return new(link.ParserId, link.Id, link.UrlInfo, link.Statistics, link.Active);
	}

	/// <summary>
	/// Сбросить время работы парсера.
	/// </summary>
	public void ResetWorkTime()
	{
		Statistics = Statistics.ResetWorkTime();
	}

	/// <summary>
	/// Добавить количество обработанных элементов.
	/// </summary>
	/// <param name="count">Количество элементов.</param>
	/// <returns>Результат операции.</returns>
	public Result AddParsedCount(int count)
	{
		Result<ParsingStatistics> updated = Statistics.IncreaseParsedCount(count);
		if (updated.IsFailure)
		{
			return Result.Failure(updated.Error);
		}

		Statistics = updated.Value;
		return Result.Success();
	}

	/// <summary>
	/// Добавить время работы парсера.
	/// </summary>
	/// <param name="totalElapsedSeconds">Время в секундах.</param>
	/// <returns>Результат операции.</returns>
	public Result AddWorkTime(long totalElapsedSeconds)
	{
		Result<ParsingStatistics> updated = Statistics.AddWorkTime(totalElapsedSeconds);
		if (updated.IsFailure)
		{
			return Result.Failure(updated.Error);
		}

		Statistics = updated.Value;
		return Result.Success();
	}

	/// <summary>
	/// Включить ссылку.
	/// </summary>
	public void Enable()
	{
		if (Active)
		{
			return;
		}

		Active = true;
	}

	/// <summary>
	/// Отключить ссылку.
	/// </summary>
	public void Disable()
	{
		if (!Active)
		{
			return;
		}

		Active = false;
	}

	/// <summary>
	/// Редактировать ссылку (имя или url).
	/// </summary>
	/// <param name="otherName">Новое имя.</param>
	/// <param name="otherUrl">Новый url.</param>
	/// <returns>Результат редактирования.</returns>
	public Result<SubscribedParserLink> Edit(string? otherName, string? otherUrl)
	{
		Result<SubscribedParserLinkUrlInfo> copy = UrlInfo.Copy();

		if (!string.IsNullOrWhiteSpace(otherName))
		{
			copy = copy.Value.Rename(otherName);
		}

		if (!string.IsNullOrWhiteSpace(otherUrl))
		{
			copy = copy.Value.ChangeUrl(otherUrl);
		}

		if (copy.IsFailure)
		{
			return Result.Failure<SubscribedParserLink>(copy.Error);
		}

		UrlInfo = copy.Value;
		return this;
	}
}
