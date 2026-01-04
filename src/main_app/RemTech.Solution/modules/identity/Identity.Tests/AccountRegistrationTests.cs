using Identity.Domain.Accounts.Features.RegisterAccount;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Tests;

public sealed class AccountRegistrationTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
    private IServiceProvider Services { get; } = factory.Services;

    [Fact]
    private async Task Invoke_Account_Registration_Success()
    {
        string login = "TestAccount";
        string email = "testAccount@mail.com";
        string password = "SomeSimplePassword@123";
        RegisterAccountCommand command = new(email, login, password);
        Result<Unit> result = await Services.InvokeAccountRegistration(command);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Invoke_Account_Registration_Duplicate_Email_Failure()
    {
        string login = "TestAccount";
        string email = "testAccount@mail.com";
        string password = "SomeSimplePassword@123";
        RegisterAccountCommand command1 = new(email, login, password);
        Result<Unit> result1 = await Services.InvokeAccountRegistration(command1);
        Assert.True(result1.IsSuccess);

        string otherLogin = "OtherLogin";
        RegisterAccountCommand command2 = new(email, otherLogin, password);
        Result<Unit> result2 = await Services.InvokeAccountRegistration(command2);
        Assert.True(result2.IsFailure);
    }
    
    [Fact]
    private async Task Invoke_Account_Registration_Duplicate_Login_Failure()
    {
        string login = "TestAccount";
        string email = "testAccount@mail.com";
        string password = "SomeSimplePassword@123";
        RegisterAccountCommand command1 = new(email, login, password);
        Result<Unit> result1 = await Services.InvokeAccountRegistration(command1);
        Assert.True(result1.IsSuccess);

        string otherEmail = "otherAccount@mail.com";
        RegisterAccountCommand command2 = new(otherEmail, login, password);
        Result<Unit> result2 = await Services.InvokeAccountRegistration(command2);
        Assert.True(result2.IsFailure);
    }
}