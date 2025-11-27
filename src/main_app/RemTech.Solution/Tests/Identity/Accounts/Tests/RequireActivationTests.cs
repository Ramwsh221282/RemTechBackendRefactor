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
        Result<AccountResponse> result = await _facade.AddAccount.Invoke(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivationByAccount.Invoke(accountId);
        Assert.True(activationRequested.IsSuccess);
        bool hasAccountTicket = await _facade.AccountHasAccountTickets.Invoke(accountId);
        Assert.True(hasAccountTicket);
        bool hasOutboxMessageWithType = await _facade.HasIdentityOutboxWithType.Invoke(outboxMessageType);
        Assert.True(hasOutboxMessageWithType);
        await Task.Delay(TimeSpan.FromSeconds(5));
        bool hasMessageProcessed = await _facade.IsOutboxMessageWithTypeProcessed.Invoke(outboxMessageType);
        Assert.True(hasMessageProcessed);
    }

    [Fact]
    private async Task Account_Cannot_Have_More_Than_One_Require_Activation_Ticket()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount.Invoke(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.RequireActivationByAccount.Invoke(accountId);
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivationByAccount.Invoke(accountId);
        Assert.True(activationRequested.IsFailure);
    }
    
    [Fact]
    private async Task Require_Activation_Ticket_Already_Activated()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount.Invoke(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.StabAccountActivated.Invoke(accountId);
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivationByAccount.Invoke(accountId);
        Assert.True(activationRequested.IsFailure);
    }

    [Fact]
    private async Task Require_Activation_Ticket_Account_Not_Found()
    {
        Result<RequireActivationResponse> activationRequested = await _facade.RequireActivationByAccount.Invoke(Guid.NewGuid());
        Assert.True(activationRequested.IsFailure);
    }
}