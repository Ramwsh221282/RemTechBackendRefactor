using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.ParserStateManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using RemTech.SharedKernel.Configuration;

namespace Tests.ParsersControl.Features;

public sealed class HasParserState(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id)
    {
        StatefulParserQueryArgs args = new(id);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IStatefulParsersStorage storage = scope.Resolve<IStatefulParsersStorage>();
        StatefulParser? parser = await storage.Fetch(args, CancellationToken.None);
        return parser != null;
    }
}