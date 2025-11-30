namespace ParsersControl.Core.ParserStatisticsManagement;

public record ParserStatisticData(
    Guid ParserId, 
    int Processed, 
    long TotalSecondsElapsed);