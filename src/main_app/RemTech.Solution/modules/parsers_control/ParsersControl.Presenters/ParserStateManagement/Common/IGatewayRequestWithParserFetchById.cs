using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;

namespace ParsersControl.Presenters.ParserStateManagement.Common;

public interface IGatewayRequestWithParserFetchById
{
    public Guid Id { get; }

    public Func<IParserWorkStatesStorage, CancellationToken, Task<ParserWorkTurner?>> FetchMethod()
    {
        return (storage, token) => storage.Fetch(new ParserWorkTurnerQueryArgs(Id: Id), token);
    }
}