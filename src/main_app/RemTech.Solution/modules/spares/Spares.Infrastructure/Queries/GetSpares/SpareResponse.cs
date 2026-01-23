namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class SpareResponse
{
	public required Guid Id { get; set; }
	public required string Url { get; set; }
	public required long Price { get; set; }
	public required string Oem { get; set; }
	public required string Text { get; set; }
	public required string Type { get; set; }
	public required bool IsNds { get; set; }
	public required string Location { get; set; }
}
