namespace Spares.Domain.Models;

public sealed record SpareDetails(
    SpareOem Oem,
    SpareTextInfo Text,
    SparePrice Price,
    SpareType Type,
    SpareAddress Address,
    SparePhotoCollection Photos
);
