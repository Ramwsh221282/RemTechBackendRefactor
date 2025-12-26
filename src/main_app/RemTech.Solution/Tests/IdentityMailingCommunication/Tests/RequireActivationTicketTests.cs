using Identity.Gateways.Accounts.RequireActivation;
using Identity.Gateways.Accounts.Responses;
using Identity.Gateways.AccountTickets.OnAccountTicketActivationRequired;
using Mailing.Presenters.Mailers.AddMailer;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.CommonFeatures.Accounts;
using Tests.Identity.CommonFeatures.IdentityOutbox;
using Tests.Mailing.Features;

namespace Tests.IdentityMailingCommunication.Tests;

public sealed class RequireActivationTicketTests(
    IdentityMailingCommunicationFixture fixture
    )
    : IClassFixture<IdentityMailingCommunicationFixture>
{
    private readonly MailingModuleFeaturesFacade _mailingFeatures = new(fixture.Services);
    private readonly AddAccount _addAccount = new(fixture.Services);
    private readonly RequireActivationByAccount _requireActivationByAccount = new(fixture.Services);
    private readonly IsOutboxMessageWithTypeProcessed _isOutboxMessageWithTypeProcessed = new(fixture.Services);
    
    [Fact]
    private async Task Ensure_Has_Pending_Inbox_Message()
    {
        const string mailingEmail = "rem_tech_demo@mail.ru";
        const string mailingPassword = "RhUXBTF78WeXYk6ehELq";
        const string identityEmail = "jimkrauz@gmail.com";
        const string identityName = "someName";
        const string identityPassword = "somePassword";
        const string outboxMessageType = AddAccountTicketOnAccountActivationRequested.Type;
        
        Result<AddMailerResponse> mailerCreation = await _mailingFeatures.AddMailer(mailingEmail, mailingPassword);
        Assert.True(mailerCreation.IsSuccess);
        
        Result<AccountResponse> account = await _addAccount.Invoke(identityName, identityEmail, identityPassword);
        Assert.True(account.IsSuccess);
        Guid accountId = account.Value.Id;
        
        Result<RequireActivationResponse> activationRequested = await _requireActivationByAccount.Invoke(accountId);
        Assert.True(activationRequested.IsSuccess);
        
        await Task.Delay(TimeSpan.FromSeconds(10));
        bool hasMessageProcessed = await _isOutboxMessageWithTypeProcessed.Invoke(outboxMessageType);
        Assert.True(hasMessageProcessed);

        bool hasInboxMessage = await _mailingFeatures.EnsureHasInboxMessageProcessed();
        Assert.True(hasInboxMessage);
    }
}