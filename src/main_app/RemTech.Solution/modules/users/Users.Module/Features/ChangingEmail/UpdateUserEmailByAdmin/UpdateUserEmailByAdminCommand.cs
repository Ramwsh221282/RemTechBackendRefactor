using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.ChangingEmail.UpdateUserEmailByAdmin;

internal sealed record UpdateUserEmailByAdminCommand(string NewEmail, Guid UserId) : ICommand;
