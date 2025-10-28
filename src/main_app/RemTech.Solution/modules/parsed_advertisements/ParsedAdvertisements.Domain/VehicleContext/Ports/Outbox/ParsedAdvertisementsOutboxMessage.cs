namespace ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;

public sealed class ParsedAdvertisementsOutboxMessage
{
    public const string Pending = "Pending";
    public const string Success = "Success";

    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required int Retries { get; set; }
    public required string? LastError { get; set; }
    public required string Content { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
}