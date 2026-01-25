using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.StartParserWork;

public sealed record StartParserCommand(Guid Id) : ICommand;
