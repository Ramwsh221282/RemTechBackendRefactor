namespace Mailers.Application.Features.PingMailer;

public sealed record PingMailerArgs(Guid Id, string To, CancellationToken Ct);