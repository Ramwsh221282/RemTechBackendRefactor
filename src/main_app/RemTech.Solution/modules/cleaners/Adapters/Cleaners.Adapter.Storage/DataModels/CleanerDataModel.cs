namespace Cleaners.Adapter.Storage.DataModels;

public sealed class CleanerDataModel : ICleanerDataModel
{
    public required Guid Id { get; set; }
    public required int CleanedAmount { get; set; }
    public required DateTime? LastRun { get; set; }
    public required DateTime? NextRun { get; set; }
    public required int WaitDays { get; set; }
    public required string State { get; set; }
    public required int Hours { get; set; }
    public required int Minutes { get; set; }
    public required int Seconds { get; set; }
    public required int ItemsDateDayThreshold { get; set; }
}
