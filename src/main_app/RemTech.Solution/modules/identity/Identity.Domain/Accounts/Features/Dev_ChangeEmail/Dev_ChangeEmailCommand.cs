using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Dev_ChangeEmail;

public sealed record Dev_ChangeEmailCommand(
	string NewEmail,
	Guid? AccountId = null,
	string? Login = null,
	string? Email = null
) : ICommand;
