namespace Cleaners.Adapter.Storage.DataModels;

public interface ICleanerDataModel
{
    public Guid Id { get; set; }
    public int CleanedAmount { get; set; }
    public DateTime? LastRun { get; set; }
    public DateTime? NextRun { get; set; }
    public int WaitDays { get; set; }
    public string State { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public int ItemsDateDayThreshold { get; set; }
}