using RemTech.SharedKernel.Configuration;

namespace Tests.ParsersControl.Features;

public sealed class EnsureParserExists(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id)
    {
        CancellationToken ct = CancellationToken.None;
        return await sp.ExecuteInScope<IRegisteredParsersStorage, bool>(async storage =>
        {
            RegisteredParserQuery query = new(Id: id);
            RegisteredParser? parser = await storage.Fetch(query, ct);
            return parser != null;
        });
    }
}