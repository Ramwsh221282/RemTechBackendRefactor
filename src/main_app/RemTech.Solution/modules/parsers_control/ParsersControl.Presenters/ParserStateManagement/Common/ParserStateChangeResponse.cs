using ParsersControl.Core.ParserWorkStateManagement;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStateManagement.Common;

public sealed class ParserStateChangeResponse : IResponse
{
    public Guid Id { get; private set; } = Guid.Empty;
    public string State { get; private set; } = string.Empty;
    private void AddId(Guid id) =>  Id = id;
    private void AddState(string state) => State = state;
    public ParserStateChangeResponse(ParserWorkTurner turner) => turner.Write(AddId, AddState);
}