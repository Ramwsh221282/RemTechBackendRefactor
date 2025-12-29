using Microsoft.Extensions.DependencyInjection;

namespace Tests.ParsersControl.Features;

public sealed class HasParserState(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id)
    {
        ParserWorkTurnerQueryArgs args = new(id);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IParserWorkStatesStorage storage = scope.Resolve<IParserWorkStatesStorage>();
        ParserWorkTurner? parser = await storage.Fetch(args, CancellationToken.None);
        return parser != null;
    }
}