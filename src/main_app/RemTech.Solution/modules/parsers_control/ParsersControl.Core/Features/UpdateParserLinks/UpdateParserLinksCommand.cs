using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Features.UpdateParserLinks;

public record UpdateParserLinksCommand(Guid ParserId, IEnumerable<UpdateParserLinksCommandInfo> UpdateParameters)
	: ICommand;
