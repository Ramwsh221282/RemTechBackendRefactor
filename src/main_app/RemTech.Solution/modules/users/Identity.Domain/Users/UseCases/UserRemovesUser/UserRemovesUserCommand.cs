using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.UserRemovesUser;

public sealed record UserRemovesUserCommand(Guid RemoverId, Guid RemovalId) : ICommand;
