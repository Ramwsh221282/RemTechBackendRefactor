using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ChangingEmail;

public sealed record ChangeEmailCommand(Guid ChangerId, string NewEmail) : ICommand;
