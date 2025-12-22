using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.StopParserWork;

public sealed record StopParserWorkCommand(Guid Id) : ICommand;