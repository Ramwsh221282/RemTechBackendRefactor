using Identity.Integrational.Tests.Common;
using Identity.Integrational.Tests.Common.Cases;

namespace Identity.Integrational.Tests.CreatePasswordResetTicket;

public class CreatePasswordResetTicketTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    private UserCaseResult _user = null!;

    [Fact]
    private async Task User_creates_reset_password_ticket_by_email()
    {
        var result = await UseCases.CreatePasswordResetTicket(email: _user.Email);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task User_creates_reset_password_ticket_by_login()
    {
        var result = await UseCases.CreatePasswordResetTicket(login: _user.Login);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task User_creates_reset_password_without_providing_email_or_login()
    {
        var result = await UseCases.CreatePasswordResetTicket();
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Not_confirmed_user_creates_reset_password_ticket()
    {
        var user = await new UserCreationCase(
            login: "notConfirmed",
            email: "notConfirmed@mail.com",
            password: "notConfirmedPassword!23"
        ).Invoke(this);

        var result = await UseCases.CreatePasswordResetTicket(email: user.Email, login: user.Login);
        Assert.True(result.IsFailure);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _user = await new UserCreationCase(
            "userLogin",
            "userEmail@mail.com",
            "userPassword!23"
        ).Invoke(this);
        await PrepareUserForTests(_user);
    }

    private async Task PrepareUserForTests(UserCaseResult user)
    {
        Guid ticketId = await CreateEmailConfirmationTicket(user);
        await ConfirmEmailConfirmationTicket(ticketId);
    }

    private async Task<Guid> CreateEmailConfirmationTicket(UserCaseResult user)
    {
        Guid id = user.Id;
        string password = user.Password;
        var withTicket = await UseCases.CreateEmailConfirmationUseCase(id, password);
        if (withTicket.IsFailure)
            throw new ApplicationException("Could not create email confirmation ticket.");
        return withTicket.Value.Tickets.Tickets.First().Id.Id;
    }

    private async Task ConfirmEmailConfirmationTicket(Guid ticketId)
    {
        var confirmation = await UseCases.ConfirmEmailTicketUseCase(ticketId);
        if (confirmation.IsFailure)
            throw new ApplicationException("Could not confirm email ticket.");
    }
}
