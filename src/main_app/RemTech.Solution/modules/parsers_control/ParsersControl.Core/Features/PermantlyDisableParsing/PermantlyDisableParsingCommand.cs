using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.PermantlyDisableParsing;

public sealed record PermantlyDisableParsingCommand(Guid Id) : ICommand;
