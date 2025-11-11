namespace Mailers.Application.Features.CreateMailer;

public sealed record CreateMailerArgs(
    string Email,
    string SmtpPassword,
    CancellationToken Ct);