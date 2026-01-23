using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Domain.Features;

public sealed record AddSparesCommand(AddSparesCreatorPayload Creator, IEnumerable<AddSpareCommandPayload> Spares)
	: ICommand;
