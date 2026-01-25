using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyStartParsing;

public sealed record PermantlyStartParsingCommand(Guid Id) : ICommand;
