using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Domain.Features;

/// <summary>
/// Команда для добавления запчастей.
/// </summary>
/// <param name="Creator">Создатель команды.</param>
/// <param name="Spares">Коллекция запчастей для добавления.</param>
public sealed record AddSparesCommand(AddSparesCreatorPayload Creator, IEnumerable<AddSpareCommandPayload> Spares)
	: ICommand;
