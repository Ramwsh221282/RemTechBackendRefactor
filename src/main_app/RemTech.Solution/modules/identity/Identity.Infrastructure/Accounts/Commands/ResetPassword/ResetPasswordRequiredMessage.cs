namespace Identity.Infrastructure.Accounts.Commands.ResetPassword;

public sealed record ResetPasswordRequiredMessage(
	Guid AccountId,
	string AccountEmail,
	Guid TicketId,
	string TicketPurpose
);
