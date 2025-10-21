namespace Identity.Domain.Users.UseCases.CheckRoot;

public sealed record CheckRootResponse(IEnumerable<User> Users);
