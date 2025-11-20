using Tickets.Core.Snapshots;

namespace Tickets.Core;

public sealed class TicketMetadata
{
    private const int MaxTicketTypeLength = 128;
    private readonly Guid _creatorId;
    private readonly Guid _ticketId;
    private readonly string _type;
    private readonly string? _extraInformationJson;

    public bool IsCreatedBy(Guid creatorId) =>
        _creatorId == creatorId;
    
    public bool IsSameType(string type) =>
        _type == type;

    public TicketMetadataSnapshot Snapshot()
    {
        return new TicketMetadataSnapshot(_creatorId, _ticketId, _type, _extraInformationJson);
    }
    
    private TicketMetadata(Guid creatorId, Guid ticketId, string type, string? extraInformationJson) =>
        (_creatorId, _ticketId, _type, _extraInformationJson) = (creatorId, ticketId, type, extraInformationJson);

    public static Result<TicketMetadata> Create(Guid creatorId, Guid ticketId, string type, string? extraInformationJson)
    {
        if (creatorId == Guid.Empty) return Validation("Идентификатор создателя заявки пустой.");
        if (ticketId == Guid.Empty) return Validation("Идентификатор заявки пустой.");
        if (string.IsNullOrWhiteSpace(type)) return Validation("Тип заявки пустой.");
        if (type.Length > MaxTicketTypeLength) return Validation("Тип заявки невалиден.");
        return new TicketMetadata(creatorId, ticketId, type, extraInformationJson);
    }
}