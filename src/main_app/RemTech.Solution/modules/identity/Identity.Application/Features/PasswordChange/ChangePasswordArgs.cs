namespace Identity.Application.Features.PasswordChange;

public sealed record ChangePasswordArgs(Guid Id, string NextPassword, CancellationToken Ct);