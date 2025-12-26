using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.StopParserWork;

public sealed record StopParserWorkCommand(Guid Id) : ICommand;