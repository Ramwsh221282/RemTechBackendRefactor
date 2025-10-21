using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmation;

public sealed record CreateEmailConfirmationCommand(Guid UserId) : ICommand;
