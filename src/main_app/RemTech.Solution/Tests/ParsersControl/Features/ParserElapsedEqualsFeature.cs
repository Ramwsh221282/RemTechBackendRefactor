using Microsoft.Extensions.DependencyInjection;

namespace Tests.ParsersControl.Features;

public sealed class ParserElapsedEqualsFeature(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id, long elapsedSeconds)
    {
        CancellationToken ct = CancellationToken.None;
        ParserStatisticsQuery query = new(id);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IParserStatisticsStorage storage = scope.Resolve<IParserStatisticsStorage>();
        ParserStatistic? statistic = await storage.Fetch(query, ct);
        if (statistic == null) throw new InvalidOperationException("Parser was not found.");
        return statistic.ElapsedEqualsTo(elapsedSeconds);
    }
}