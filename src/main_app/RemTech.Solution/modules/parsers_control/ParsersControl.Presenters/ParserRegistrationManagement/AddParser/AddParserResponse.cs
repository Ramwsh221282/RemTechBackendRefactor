using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserRegistrationManagement.AddParser;

public class AddParserResponse : IResponse, IOnParserRegisteredEventListener
{
    public Guid Id { get; private set; } = Guid.Empty;
    
    private void AddId(Guid id) => Id = id;
    
    public Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        new RegisteredParser(data).Write(AddId);
        return Task.FromResult(Result.Success(Unit.Value));
    }
}