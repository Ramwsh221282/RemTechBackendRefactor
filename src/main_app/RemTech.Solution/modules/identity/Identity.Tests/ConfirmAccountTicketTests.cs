using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

public sealed class ConfirmAccountTicketTests : IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; }

	public ConfirmAccountTicketTests(IntegrationalTestsFactory factory)
	{
		Services = factory.Services;
	}

	[Fact]
	private async Task Confirm_Account_Ticket_Success()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command = new(email, login, password);
		Result<Unit> result = await Services.InvokeAccountRegistration(command);
		Assert.True(result.IsSuccess);
		Result<AccountTicket> ticketResult = await Services.GetTicketOfPurpose(
			AccountTicketPurposes.EmailConfirmationRequired
		);
		Assert.True(ticketResult.IsSuccess);
		Result<Unit> confirmationResult = await Services.ConfirmAccountTicket(
			ticketResult.Value.AccountId.Value,
			ticketResult.Value.TicketId
		);
		Assert.True(confirmationResult.IsSuccess);
	}

	[Fact]
	private async Task Confirm_Account_Ticket_Ensure_Ticket_Is_Confirmed()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command = new(email, login, password);
		Result<Unit> result = await Services.InvokeAccountRegistration(command);
		Assert.True(result.IsSuccess);
		Result<AccountTicket> ticketResult = await Services.GetTicketOfPurpose(
			AccountTicketPurposes.EmailConfirmationRequired
		);
		Assert.True(ticketResult.IsSuccess);
		Result<Unit> confirmationResult = await Services.ConfirmAccountTicket(
			ticketResult.Value.AccountId.Value,
			ticketResult.Value.TicketId
		);
		Assert.True(confirmationResult.IsSuccess);

		Result<AccountTicket> confirmedTicket = await Services.GetTicketOfPurpose(
			AccountTicketPurposes.EmailConfirmationRequired
		);
		Assert.True(ticketResult.IsSuccess);
		Assert.True(confirmedTicket.Value.Finished);
	}

	[Fact]
	private async Task Try_To_Confirm_Ticket_Not_Belong_To_Account_Failure()
	{
		const string login1 = "TestAccount";
		const string email1 = "testAccount@mail.com";
		const string password1 = "SomeSimplePassword@123";

		const string login2 = "TestAccount2";
		const string email2 = "testAccount2@mail.com";
		const string password2 = "SomeSimplePassword@123";

		RegisterAccountCommand command1 = new(email1, login1, password1);
		RegisterAccountCommand command2 = new(email2, login2, password2);

		Result<Unit> result1 = await Services.InvokeAccountRegistration(command1);
		Assert.True(result1.IsSuccess);
		Result<AccountTicket> ticketResult = await Services.GetTicketOfPurpose(
			AccountTicketPurposes.EmailConfirmationRequired
		);
		Assert.True(ticketResult.IsSuccess);
		Result<Unit> result2 = await Services.InvokeAccountRegistration(command2);
		Assert.True(result2.IsSuccess);

		Result<Account> account = await Services.GetAccountByEmail(email2);
		Assert.True(account.IsSuccess);

		Result<Unit> confirmationResult = await Services.ConfirmAccountTicket(
			account.Value.Id.Value,
			ticketResult.Value.TicketId
		);
		Assert.True(confirmationResult.IsFailure);
	}
}
