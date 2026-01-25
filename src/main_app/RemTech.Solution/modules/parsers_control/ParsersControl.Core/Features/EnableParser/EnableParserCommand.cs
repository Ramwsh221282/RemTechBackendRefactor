using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.EnableParser;

public sealed record EnableParserCommand(Guid Id) : ICommand;
