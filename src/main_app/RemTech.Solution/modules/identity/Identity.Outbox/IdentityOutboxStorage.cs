using Identity.Outbox.Delegates;

namespace Identity.Outbox;

public sealed record IdentityOutboxStorage(
    Add Add, 
    HasAny HasAny, 
    GetPendingMessages GetPendingMessages, 
    RemoveMany RemoveMany, 
    UpdateMany UpdateMany);