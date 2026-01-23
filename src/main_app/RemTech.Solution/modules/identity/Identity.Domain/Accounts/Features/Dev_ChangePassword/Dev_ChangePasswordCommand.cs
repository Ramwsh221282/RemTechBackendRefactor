using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Dev_ChangePassword;

public sealed record Dev_ChangePasswordCommand(
	string NewPassword,
	Guid? AccountId = null,
	string? AccountLogin = null,
	string? AccountEmail = null
) : ICommand;
