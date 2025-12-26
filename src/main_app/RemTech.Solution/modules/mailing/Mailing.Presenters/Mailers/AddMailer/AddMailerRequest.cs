using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Presenters.Mailers.AddMailer;

public sealed record AddMailerRequest(string Password, string Email, CancellationToken Ct) : IRequest;