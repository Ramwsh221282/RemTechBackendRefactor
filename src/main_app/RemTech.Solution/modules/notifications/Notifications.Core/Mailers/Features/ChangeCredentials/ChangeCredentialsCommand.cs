using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.ChangeCredentials;

public sealed record ChangeCredentialsCommand(Guid Id, string SmtpPassword, string Email) : ICommand;