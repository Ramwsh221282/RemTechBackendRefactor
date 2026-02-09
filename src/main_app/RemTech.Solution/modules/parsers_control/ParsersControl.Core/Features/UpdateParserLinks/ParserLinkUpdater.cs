using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Features.UpdateParserLinks;

/// <summary>
/// Обновитель ссылки парсера.
/// </summary>
public sealed class ParserLinkUpdater
{
	private ParserLinkUpdater(SubscribedParserLinkId id, bool? activity, string? name, string? url)
	{
		Id = id;
		Activity = activity;
		Name = name;
		Url = url;
	}

	/// <summary>
	/// Идентификатор ссылки.
	/// </summary>
	public SubscribedParserLinkId Id { get; }

	/// <summary>
	/// Признак активности.
	/// </summary>
	public bool? Activity { get; }

	/// <summary>
	/// Имя ссылки.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	/// 	URL ссылки.
	/// </summary>
	public string? Url { get; }

	/// <summary>
	/// Создаёт обновитель ссылки парсера.
	/// </summary>
	/// <param name="id">Идентификатор ссылки парсера.</param>
	/// <param name="activity">Признак активности ссылки парсера.</param>
	/// <param name="name">Имя ссылки парсера.</param>
	/// <param name="url">URL ссылки парсера.</param>
	/// <returns>Результат создания обновителя ссылки парсера.</returns>
	public static Result<ParserLinkUpdater> Create(Guid id, bool? activity, string? name, string? url)
	{
		Result<SubscribedParserLinkId> idResult = SubscribedParserLinkId.From(id);
		return idResult.IsFailure ? idResult.Error : new ParserLinkUpdater(idResult.Value, activity, name, url);
	}

	/// <summary>
	/// Проверяет, что обновитель принадлежит указанной ссылке парсера.
	/// </summary>
	/// <param name="link">Ссылка парсера для проверки принадлежности.</param>
	/// <returns>True, если обновитель принадлежит указанной ссылке парсера; в противном случае false.</returns>
	public bool UpdateBelongsTo(SubscribedParserLink link)
	{
		return Id == link.Id;
	}

	/// <summary>
	/// Обновляет ссылку парсера.
	/// </summary>
	/// <param name="link">Ссылка парсера для обновления.</param>
	/// <returns>Результат обновления ссылки парсера.</returns>
	public Result Update(SubscribedParserLink link)
	{
		Result<SubscribedParserLink> editing = link.Edit(Name, Url);
		if (editing.IsFailure)
		{
			return Result.Failure(editing.Error);
		}

		if (Activity.HasValue)
		{
			if (Activity.Value)
			{
				link.Enable();
			}
			else
			{
				link.Disable();
			}
		}

		return editing;
	}
}
