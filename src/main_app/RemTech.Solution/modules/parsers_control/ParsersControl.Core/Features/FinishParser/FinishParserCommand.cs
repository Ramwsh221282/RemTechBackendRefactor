using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.FinishParser;

public sealed record FinishParserCommand(Guid Id, long TotalElapsedSeconds) : ICommand;