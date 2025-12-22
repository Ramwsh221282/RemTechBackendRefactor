using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.StartParserWork;

public sealed record StartParserCommand(Guid Id) : ICommand;