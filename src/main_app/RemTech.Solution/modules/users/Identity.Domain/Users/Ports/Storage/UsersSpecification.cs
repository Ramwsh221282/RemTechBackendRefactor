namespace Identity.Domain.Users.Ports.Storage;

public sealed record UsersSpecification(
    string? Login = null,
    string? Email = null,
    IEnumerable<string>? Roles = null,
    IEnumerable<string>? OrderClauses = null,
    bool? VerifiedOnly = null,
    string OrderMode = "ASC",
    int Page = 1,
    int PageSize = 20
);
