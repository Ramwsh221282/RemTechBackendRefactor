namespace Identity.Core.SubjectsModule.Domain.Tickets;

public readonly record struct SubjectTicketSnapshot(Optional<Guid> CreatorId, Guid Id, string Type, bool Active);