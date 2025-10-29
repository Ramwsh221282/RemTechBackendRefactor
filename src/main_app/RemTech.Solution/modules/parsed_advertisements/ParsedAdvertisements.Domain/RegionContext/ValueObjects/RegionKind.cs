using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.RegionContext.ValueObjects;

public sealed record RegionKind
{
    public const int MaxLength = 100;
    public string Value { get; }

    private RegionKind(string value) => Value = value;

    public static Status<RegionKind> Create(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind))
            return Error.Validation("Пустой тип локации.");

        if (kind.Length > RegionName.MaxLength)
            return Error.Validation($"Тип локации превышает длину: {MaxLength} символов.");

        return new RegionKind(kind);
    }
}