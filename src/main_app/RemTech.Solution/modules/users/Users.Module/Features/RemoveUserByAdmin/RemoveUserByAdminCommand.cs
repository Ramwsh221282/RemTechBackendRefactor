using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Models;

namespace Users.Module.Features.RemoveUserByAdmin;

internal sealed record RemoveUserByAdminCommand(UserJwt Jwt, Guid UserId) : ICommand;
