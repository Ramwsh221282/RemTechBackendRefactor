namespace Tickets.Core;

public sealed class TicketMetadata
{
    private const int MaxTicketTypeLength = 128;
    private readonly Guid _creatorId;
    private readonly Guid _ticketId;
    private readonly string _type;

    public bool IsCreatedBy(Guid creatorId) =>
        _creatorId == creatorId;
    
    public bool IsSameType(string type) =>
        _type == type;

    public TicketMetadataSnapshot Snapshot()
    {
        return new TicketMetadataSnapshot(_creatorId, _ticketId, _type);
    }
    
    private TicketMetadata(Guid creatorId, Guid ticketId, string type) =>
        (_creatorId, _ticketId, _type) = (creatorId, ticketId, type);

    public static Result<TicketMetadata> Create(Guid creatorId, Guid ticketId, string type)
    {
        if (creatorId == Guid.Empty) return Validation("Идентификатор создателя заявки пустой.");
        if (ticketId == Guid.Empty) return Validation("Идентификатор заявки пустой.");
        if (string.IsNullOrWhiteSpace(type)) return Validation("Тип заявки пустой.");
        if (type.Length > MaxTicketTypeLength) return Validation("Тип заявки невалиден.");
        return new TicketMetadata(creatorId, ticketId, type);
    }
}