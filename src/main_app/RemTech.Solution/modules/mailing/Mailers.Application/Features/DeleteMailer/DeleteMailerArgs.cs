namespace Mailers.Application.Features.DeleteMailer;

public sealed record DeleteMailerArgs(Guid Id, CancellationToken Ct);