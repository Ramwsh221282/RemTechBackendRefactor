using Identity.Integrational.Tests.Common;
using Identity.Integrational.Tests.Common.Cases;

namespace Identity.Integrational.Tests.CreateConfirmationTicket;

public class CreateConfirmationTicketTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    private UserCaseResult _user = null!;

    [Fact]
    private async Task User_creates_email_confirmation_ticket()
    {
        var confirmation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(confirmation.IsSuccess);
    }

    [Fact]
    private async Task Ensure_user_has_ticket_after_creation()
    {
        var confirmation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(confirmation.IsSuccess);
        Assert.True(confirmation.Value.Tickets.Tickets.Any());

        var fromDb = await UseCases.GetUserUseCase(_user.Id);
        Assert.NotNull(fromDb);
        Assert.True(fromDb.Tickets.Tickets.Any());
    }

    [Fact]
    private async Task Unauthorized_user_tries_create_email_confirmation_ticket()
    {
        string password = "notRealPassword!23;";
        var confirmation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, password);
        Assert.True(confirmation.IsFailure);
    }

    [Fact]
    private async Task User_with_invalid_id_tries_create_email_confirmation_ticket()
    {
        Guid id = Guid.Empty;
        var confirmation = await UseCases.CreateEmailConfirmationUseCase(id, _user.Password);
        Assert.True(confirmation.IsFailure);
    }

    [Fact]
    private async Task Not_existing_user_tries_create_email_confirmation_ticket()
    {
        Guid id = Guid.NewGuid();
        var confirmation = await UseCases.CreateEmailConfirmationUseCase(id, _user.Password);
        Assert.True(confirmation.IsFailure);
    }

    [Fact]
    private async Task User_with_confirmed_email_tries_create_email_confirmation_ticket()
    {
        var creation = await UseCases.CreateEmailConfirmationUseCase(_user.Id, _user.Password);
        Assert.True(creation.IsSuccess);
        var ticketId = creation.Value.Tickets.Tickets.First().Id.Id;
        var confirmation = await UseCases.ConfirmEmailTicketUseCase(ticketId);
        Assert.True(confirmation.IsSuccess);

        var ticketCreation = await UseCases.CreateEmailConfirmationUseCase(
            _user.Id,
            _user.Password
        );

        Assert.True(ticketCreation.IsFailure);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await new UserCreationCase(
            "userLogin",
            "userEmail@mail.com",
            "userPassword!23"
        ).Invoke(this);
    }
}
