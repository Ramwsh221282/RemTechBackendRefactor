namespace Mailers.Application.Features.ChangeMailerSmtpPassword;

public sealed record ChangeMailerSmtpPasswordArgs(
    Guid Id,
    string NextPassword,
    CancellationToken Ct = default);