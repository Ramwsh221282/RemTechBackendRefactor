using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.ParserStatisticsManagement;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using RemTech.SharedKernel.Configuration;

namespace Tests.ParsersControl.Features;

public sealed class ParserProcessedEqualsFeature(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id, int processed)
    {
        CancellationToken ct = CancellationToken.None;
        ParserStatisticsQuery query = new(id);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IParserStatisticsStorage storage = scope.Resolve<IParserStatisticsStorage>();
        ParserStatistic? statistic = await storage.Fetch(query, ct);
        if (statistic == null) throw new InvalidOperationException("Parser was not found.");
        return statistic.ProcessedEqualsTo(processed);
    }
}