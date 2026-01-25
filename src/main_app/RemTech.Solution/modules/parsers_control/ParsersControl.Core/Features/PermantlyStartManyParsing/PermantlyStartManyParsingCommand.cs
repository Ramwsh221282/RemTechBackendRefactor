using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyStartManyParsing;

public sealed record PermantlyStartManyParsingCommand(IEnumerable<Guid> Identifiers) : ICommand;
