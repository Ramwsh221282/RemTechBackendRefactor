using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.DisableParser;

public sealed record DisableParserCommand(Guid Id) : ICommand;