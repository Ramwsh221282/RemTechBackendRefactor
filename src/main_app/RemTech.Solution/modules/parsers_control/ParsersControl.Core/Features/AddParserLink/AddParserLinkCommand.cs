using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.AddParserLink;

public sealed record AddParserLinkCommand(
    Guid ParserId,
    string LinkUrl,
    string LinkName
    ) : ICommand;