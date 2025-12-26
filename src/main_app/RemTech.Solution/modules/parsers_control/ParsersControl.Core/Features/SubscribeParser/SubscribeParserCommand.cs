using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.SubscribeParser;

public sealed record SubscribeParserCommand(Guid Id, string Domain, string Type) : ICommand;