using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Tickets;
using Identity.Tests.Fakes;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

/// <summary>
/// Тесты интеграции для регистрации аккаунта.
/// </summary>
/// <param name="factory">Фабрика для создания интеграционных тестов.</param>
public sealed class AccountRegistrationTests(IntegrationalTestsFactory factory)
	: IClassFixture<IntegrationalTestsFactory>
{
	private IServiceProvider Services { get; } = factory.Services;

	[Fact]
	private async Task Invoke_Account_Registration_Success()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command = new(email, login, password);
		Result<Unit> result = await Services.InvokeAccountRegistration(command);
		Assert.True(result.IsSuccess);
	}

	[Fact]
	private async Task Invoke_Account_Registration_Success_Ensure_Email_Confirmation_Ticket_Created()
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
	}

	[Fact]
	private async Task Invoke_Account_Registration_Success_Ensure_Consumer_Received_Message()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command = new(email, login, password);
		Result<Unit> result = await Services.InvokeAccountRegistration(command);
		Assert.True(result.IsSuccess);
		await Task.Delay(TimeSpan.FromSeconds(5));
		Assert.Equal(1, FakeOnUserAccountRegisteredConsumer.Received);
	}

	[Fact]
	private async Task Invoke_Account_Registration_Success_Ensure_Outbox_Message_Created()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command = new(email, login, password);
		Result<Unit> result = await Services.InvokeAccountRegistration(command);
		Assert.True(result.IsSuccess);
		IdentityOutboxMessage[] messages = await Services.GetOutboxMessagesOfType(
			AccountOutboxMessageTypes.NewAccountCreated
		);
		Assert.NotEmpty(messages);
		IdentityOutboxMessage? message = messages.FirstOrDefault(m =>
			m.Type == AccountOutboxMessageTypes.NewAccountCreated
		);
		Assert.NotNull(message);
	}

	[Fact]
	private async Task Invoke_Account_Registration_Duplicate_Email_Failure()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command1 = new(email, login, password);
		Result<Unit> result1 = await Services.InvokeAccountRegistration(command1);
		Assert.True(result1.IsSuccess);

		const string otherLogin = "OtherLogin";
		RegisterAccountCommand command2 = new(email, otherLogin, password);
		Result<Unit> result2 = await Services.InvokeAccountRegistration(command2);
		Assert.True(result2.IsFailure);
	}

	[Fact]
	private async Task Invoke_Account_Registration_Duplicate_Login_Failure()
	{
		const string login = "TestAccount";
		const string email = "testAccount@mail.com";
		const string password = "SomeSimplePassword@123";
		RegisterAccountCommand command1 = new(email, login, password);
		Result<Unit> result1 = await Services.InvokeAccountRegistration(command1);
		Assert.True(result1.IsSuccess);

		const string otherEmail = "otherAccount@mail.com";
		RegisterAccountCommand command2 = new(otherEmail, login, password);
		Result<Unit> result2 = await Services.InvokeAccountRegistration(command2);
		Assert.True(result2.IsFailure);
	}
}
