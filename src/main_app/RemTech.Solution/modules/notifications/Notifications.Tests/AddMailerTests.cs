using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Tests;

public sealed class AddMailerTests(IntegrationalTestsFactory factory) : IClassFixture<IntegrationalTestsFactory>
{
    private IServiceProvider Services { get; } = factory.Services;

    [Theory]
    [InlineData("testEmail@gmail.com")]
    [InlineData("testEmail@yandex.ru")]
    [InlineData("testEmail@mail.ru")]
    private async Task Add_Mailer_Success(string email)
    {
        string password = "testPassword";
        Result<Unit> result = await Services.AddMailer(email, password);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Mailer_Unsupported_SmtpHost()
    {
        string email = "testEmail@unsupported.com";
        string password = "testPassword";
        Result<Unit> result = await Services.AddMailer(email, password);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Add_Mailer_Duplicate_Email_Failure()
    {
        string email = "testEmail@gmail.com";
        string password = "testPassword";
        Result<Unit> result1 = await Services.AddMailer(email, password);
        Assert.True(result1.IsSuccess);
        
        string otherPassword = "otherPassword@123";
        Result<Unit> result2 = await Services.AddMailer(email, otherPassword);
        Assert.True(result2.IsFailure);
    }
}