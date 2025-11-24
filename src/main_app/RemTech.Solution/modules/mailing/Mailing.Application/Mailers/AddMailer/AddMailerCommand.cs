using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Application.Mailers.AddMailer;

public sealed record AddMailerCommand(string Email, string Password, CancellationToken Ct) : ICommand;