using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyDisableManyParsing;

public sealed record PermantlyDisableManyParsingCommand(IEnumerable<Guid> Identifiers) : ICommand;