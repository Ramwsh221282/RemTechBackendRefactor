using Identity.Integrational.Tests.Common;
using Identity.Integrational.Tests.Common.Cases;

namespace Identity.Integrational.Tests.ConfirmEmailTicket;

public sealed class ConfirmEmailTicketTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    private UserCaseResult _user = null!;

    [Fact]
    private async Task User_confirms_existing_email_ticket()
    {
        var creation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(creation.IsSuccess);

        var ticketId = creation.Value.Tickets.Tickets.First().Id.Id;
        var confirmation = await UseCases.ConfirmEmailTicketUseCase(ticketId);
        Assert.True(confirmation.IsSuccess);
    }

    [Fact]
    private async Task User_confirms_not_existing_ticket()
    {
        var creation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(creation.IsSuccess);
        var confirmation = await UseCases.ConfirmEmailTicketUseCase(Guid.NewGuid());
        Assert.True(confirmation.IsFailure);
    }

    [Fact]
    private async Task User_confirms_bad_id_ticket()
    {
        var creation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(creation.IsSuccess);
        var confirmation = await UseCases.ConfirmEmailTicketUseCase(Guid.Empty);
        Assert.True(confirmation.IsFailure);
    }

    [Fact]
    private async Task User_confirms_confirmed_ticket()
    {
        var creation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(creation.IsSuccess);
        var ticketId = creation.Value.Tickets.Tickets.First().Id.Id;
        await UseCases.ConfirmEmailTicketUseCase(ticketId);
        var confirmation = await UseCases.ConfirmEmailTicketUseCase(ticketId);
        Assert.True(confirmation.IsFailure);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _user = await new UserCreationCase(
            "userLogin",
            "userLogin@mail.com",
            "userPassword!23"
        ).Invoke(this);
    }
}
