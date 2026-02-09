using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.UpdateParserLinks;

/// <summary>
/// Команда обновления ссылок парсера.
/// </summary>
/// <param name="ParserId">Идентификатор парсера.</param>
/// <param name="UpdateParameters">Параметры обновления ссылок парсера.</param>
public record UpdateParserLinksCommand(Guid ParserId, IEnumerable<UpdateParserLinksCommandInfo> UpdateParameters)
	: ICommand;
