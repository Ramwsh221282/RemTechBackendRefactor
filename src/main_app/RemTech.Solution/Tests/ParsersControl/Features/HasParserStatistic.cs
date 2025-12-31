using Microsoft.Extensions.DependencyInjection;

namespace Tests.ParsersControl.Features;

public sealed class HasParserStatistic(IServiceProvider sp)
{
    public async Task<bool> Invoke(Guid id)
    {
        ParserStatisticsQuery query = new(Id: id);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IParserStatisticsStorage storage = scope.Resolve<IParserStatisticsStorage>();
        ParserStatistic? statistic = await storage.Fetch(query, CancellationToken.None);
        return statistic != null;
    }
}