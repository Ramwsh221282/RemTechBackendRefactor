using Identity.Domain.Accounts.Models;
using Identity.Domain.Tickets;

namespace Identity.Domain.Accounts.Features.ResetPassword;

public sealed record ResetPasswordResult(Guid AccountId, string AccountEmail, Guid TicketId, string TicketPurpose)
{
	public static ResetPasswordResult From(Account account, AccountTicket ticket) =>
		new(account.Id.Value, account.Email.Value, ticket.TicketId, ticket.Purpose);
}
