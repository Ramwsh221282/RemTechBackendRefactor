namespace Identity.Core.SubjectsModule.Domain.Tickets;

public sealed record SubjectTicketsStorage(Insert Insert);

public delegate Task<Result<Unit>> Insert(SubjectTicket ticket, CancellationToken ct);