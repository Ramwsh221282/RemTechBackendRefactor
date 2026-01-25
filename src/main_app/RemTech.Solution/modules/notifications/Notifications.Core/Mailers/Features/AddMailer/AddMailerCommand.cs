using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.AddMailer;

public sealed record AddMailerCommand(string SmtpPassword, string Email) : ICommand;
