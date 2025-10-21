using Identity.Domain.Users.Aggregate;

namespace Identity.Domain.Users.UseCases.CheckRoot;

public sealed record CheckRootResponse(IEnumerable<IdentityUser> Users);
