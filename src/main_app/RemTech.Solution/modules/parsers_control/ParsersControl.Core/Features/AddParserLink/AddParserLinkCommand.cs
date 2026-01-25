using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.AddParserLink;

public sealed record AddParserLinkCommand(Guid ParserId, IEnumerable<AddParserLinkCommandArg> Links) : ICommand;
