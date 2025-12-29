using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Domain.Features;

public sealed record AddSparesCommand(AddSpareCommandPayload[] Spares) : ICommand;