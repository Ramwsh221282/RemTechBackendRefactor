namespace Identity.Outbox.Delegates;

public delegate Task<bool> HasAny(CancellationToken ct);