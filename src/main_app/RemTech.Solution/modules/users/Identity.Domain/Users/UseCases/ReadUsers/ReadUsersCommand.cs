using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;

namespace Identity.Domain.Users.UseCases.ReadUsers;

public sealed record ReadUsersCommand(UsersSpecification Specification) : ICommand;
