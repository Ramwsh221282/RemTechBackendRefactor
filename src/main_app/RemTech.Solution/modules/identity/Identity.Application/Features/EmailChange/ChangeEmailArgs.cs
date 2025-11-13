namespace Identity.Application.Features.EmailChange;

public sealed record ChangeEmailArgs(Guid Id, string NextEmail, CancellationToken Ct);