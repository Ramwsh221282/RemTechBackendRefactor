using ParsersControl.Core.ParserStatisticsManagement;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserStatisticsManagement.Common;

public sealed class ParserStatisticsUpdateResponse : IResponse
{
    public Guid Id { get; private set; } = Guid.Empty;
    public int Processed { get; private set; }
    public long ElapsedSeconds { get; private set; }
    
    private void AddId(Guid id) => Id = id;
    private void AddProcessed(int processed) => Processed = processed;
    private void AddElapsedSeconds(long elapsedSeconds) => ElapsedSeconds = elapsedSeconds;
    
    public ParserStatisticsUpdateResponse(ParserStatistic statistic)
    {
        statistic.Write(AddId, AddProcessed, AddElapsedSeconds);
    }
}