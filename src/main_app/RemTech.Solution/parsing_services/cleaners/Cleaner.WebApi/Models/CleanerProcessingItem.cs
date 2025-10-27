namespace Cleaner.WebApi.Models;

public sealed class CleanerProcessingItem
{
    public const string Errored = "Errored";
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string Domain { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required bool IsDeleted { get; set; }
    public required string SourceUrl { get; set; }
    public required int ProcessedAttempts { get; set; }
    public required bool IsProcessed { get; set; }
    public string Status { get; set; } = "Enqueued";
}
