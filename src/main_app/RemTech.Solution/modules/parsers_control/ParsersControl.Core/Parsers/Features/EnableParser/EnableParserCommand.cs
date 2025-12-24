using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.EnableParser;

public sealed record EnableParserCommand(Guid Id) : ICommand;