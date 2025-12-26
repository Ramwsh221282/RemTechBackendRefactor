using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;

namespace Tests.ParsersControl.Features;

public sealed class HasParserSchedule(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid parserId)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IParserScheduleStorage storage = scope.Resolve<IParserScheduleStorage>();
        ParserScheduleQueryArgs args = new(Id: parserId);
        ParserSchedule? schedule = await storage.Fetch(args, ct);
        return schedule != null;
    }
}