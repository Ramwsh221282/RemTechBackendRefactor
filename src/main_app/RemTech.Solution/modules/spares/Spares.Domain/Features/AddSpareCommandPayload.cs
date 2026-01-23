namespace Spares.Domain.Features;

public sealed record AddSpareCommandPayload(
	Guid ContainedItemId,
	string Source,
	string Oem,
	string Title,
	long Price,
	bool IsNds,
	string Address,
	string Type,
	IEnumerable<string> PhotoPaths
);
