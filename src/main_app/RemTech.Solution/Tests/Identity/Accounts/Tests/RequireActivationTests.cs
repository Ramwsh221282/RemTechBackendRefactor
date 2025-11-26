using Identity.Gateways.Accounts.RequireActivation;
using Identity.Gateways.Accounts.Responses;
using Identity.Gateways.AccountTickets.OnAccountTicketActivationRequired;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Accounts.Fixtures;

namespace Tests.Identity.Accounts.Tests;


public sealed class RequireActivationTests(AccountsTestsFixture fixture) : IClassFixture<AccountsTestsFixture>
{
    private readonly AccountFeatureFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Require_Activation_Ticket_Success()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        const string outboxMessageType = AddAccountTicketOnAccountActivationRequested.Type;
        Result<AccountResponse> result = await _facade.AddAccount(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivation(accountId);
        Assert.True(activationRequested.IsSuccess);
        bool hasAccountTicket = await _facade.AccountHasAccountTickets(accountId);
        Assert.True(hasAccountTicket);
        bool hasOutboxMessageWithType = await _facade.HasOutboxMessageWithType(outboxMessageType);
        Assert.True(hasOutboxMessageWithType);
        await Task.Delay(TimeSpan.FromSeconds(5));
        bool hasMessageProcessed = await _facade.IsOutboxMessageWithTypeProcessed(outboxMessageType);
        Assert.True(hasMessageProcessed);
    }

    [Fact]
    private async Task Account_Cannot_Have_More_Than_One_Require_Activation_Ticket()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.RequireActivation(accountId);
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivation(accountId);
        Assert.True(activationRequested.IsFailure);
    }
    
    [Fact]
    private async Task Require_Activation_Ticket_Already_Activated()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.MakeAccountActivated(accountId);
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivation(accountId);
        Assert.True(activationRequested.IsFailure);
    }

    [Fact]
    private async Task Require_Activation_Ticket_Account_Not_Found()
    {
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivation(Guid.NewGuid());
        Assert.True(activationRequested.IsFailure);
    }
}