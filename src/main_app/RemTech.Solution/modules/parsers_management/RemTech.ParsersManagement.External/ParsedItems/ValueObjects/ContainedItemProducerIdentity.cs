using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsersManagement.External.ParsedItems.ValueObjects;

public sealed record ContainedItemProducerIdentity
{
    private readonly NotEmptyGuid _parserId;
    private readonly NotEmptyString _parserName;
    private readonly NotEmptyString _parserType;

    public ContainedItemProducerIdentity(
        NotEmptyGuid parserId,
        NotEmptyString parserName,
        NotEmptyString parserType
    )
    {
        _parserId = parserId;
        _parserName = parserName;
        _parserType = parserType;
    }

    public ContainedItemProducerIdentity(Guid? parserId, string? parserName, string? parserType)
        : this(
            new NotEmptyGuid(parserId),
            new NotEmptyString(parserName),
            new NotEmptyString(parserType)
        ) { }

    public static implicit operator bool(ContainedItemProducerIdentity identity) =>
        identity._parserId && identity._parserType && identity._parserType;

    public NotEmptyGuid SourceId() => _parserId;

    public NotEmptyString SourceName() => _parserName;

    public NotEmptyString SourceType() => _parserType;
}
