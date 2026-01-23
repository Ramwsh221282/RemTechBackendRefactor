using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Core.Contracts;

public interface IOnParserStartedListener
{
	Task Handle(SubscribedParser parser, CancellationToken ct = default);
}
