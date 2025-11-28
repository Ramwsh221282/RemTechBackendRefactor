using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;

namespace ParsersControl.Presenters.ParserStateManagement.Common;

public interface IGatewayRequestWithParserFetchById
{
    public Guid Id { get; }

    public Func<IStatefulParsersStorage, CancellationToken, Task<StatefulParser?>> FetchMethod()
    {
        return (storage, token) => storage.Fetch(new StatefulParserQueryArgs(Id: Id), token);
    }
}