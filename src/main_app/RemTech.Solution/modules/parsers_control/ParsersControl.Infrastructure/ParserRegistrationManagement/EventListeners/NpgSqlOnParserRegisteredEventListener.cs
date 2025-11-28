using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Infrastructure.ParserRegistrationManagement.NpgSql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;

public sealed class NpgSqlOnParserRegisteredEventListener(NpgSqlRegisteredParsersStorage storage) 
    : IOnParserRegisteredEventListener
{
    public async Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        RegisteredParser? withNameAndType = await storage.Fetch(new RegisteredParserQuery(
            Domain: data.Domain,
            Type: data.Type), ct);
        
        if (withNameAndType != null) return Error.Conflict($"Уже существует парсер: {data.Domain} {data.Type}");
        await storage.Persist(new RegisteredParser(data), ct);
        return Unit.Value;
    }
}