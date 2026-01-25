using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Core.Contracts;

public interface IOnParserSubscribedListener
{
	public Task Handle(SubscribedParser parser, CancellationToken ct = default);
}
