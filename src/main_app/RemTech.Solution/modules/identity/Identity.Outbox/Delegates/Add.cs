namespace Identity.Outbox.Delegates;

public delegate Task Add(IdentityOutboxMessage message, CancellationToken ct);

public delegate Task<IEnumerable<IdentityOutboxMessage>> GetPendingMessages(int MaxAmount, CancellationToken ct);

public delegate Task RemoveMany(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct);
public delegate Task UpdateMany(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct);