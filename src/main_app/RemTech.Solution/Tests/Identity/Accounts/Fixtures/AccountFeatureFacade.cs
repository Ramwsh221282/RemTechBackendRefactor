using Tests.Identity.CommonFeatures.Accounts;
using Tests.Identity.CommonFeatures.IdentityOutbox;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class AccountFeatureFacade(IServiceProvider sp)
{
    public AddAccount AddAccount => new(sp);
    public AccountCreated AccountCreated => new (sp);
    public ChangeAccountEmail ChangeAccountEmail => new(sp);
    public ChangeAccountPassword ChangeAccountPassword => new(sp);
    public ActivateAccount ActivateAccount => new(sp);
    public RequireActivationByAccount RequireActivationByAccount => new(sp);
    public HasIdentityOutboxWithType HasIdentityOutboxWithType => new(sp);
    public IsOutboxMessageWithTypeProcessed IsOutboxMessageWithTypeProcessed => new(sp);
    public GetAccount GetAccount => new(sp);
    public AccountPasswordEquals AccountPasswordEquals => new(sp);
    public StabAccountActivated StabAccountActivated => new(sp);
    public AccountHasAccountTickets AccountHasAccountTickets => new(sp);
    public RequirePasswordResetByAccount RequirePasswordResetByAccount => new(sp);
}