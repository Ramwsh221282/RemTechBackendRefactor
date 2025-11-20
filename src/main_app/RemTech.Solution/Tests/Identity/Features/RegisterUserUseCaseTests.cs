using Identity.Core.SubjectsModule.Domain.Subjects;
using Tests.ModuleFixtures;

namespace Tests.Identity.Features;

public sealed class RegisterUserUseCaseTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly IdentityModule _module = fixture.IdentityModule;

    [Fact]
    private async Task Register_User_With_Persisting_Success()
    {
        string email = "myEmail@gmail.com";
        string password = "myPassword";
        string login = "myLogin";
        Result<Subject> subject = await _module.RegisterSubject(login, email, password);
        Assert.True(subject.IsSuccess);
    }

    [Fact]
    private async Task Register_User_Duplicate_Email_Failure()
    {
        string email = "myEmail@gmail.com";
        string password = "myPassword";
        string login = "myLogin";

        await _module.RegisterSubject(login, email, password);
        Result<Subject> subject = await _module.RegisterSubject("other login", email, password);
        Assert.True(subject.IsFailure);
    }

    [Fact]
    private async Task Register_User_Duplicate_Login_Failure()
    {
        string email = "myEmail@gmail.com";
        string password = "myPassword";
        string login = "myLogin";
        await _module.RegisterSubject(login, email, password);
        Result<Subject> subject = await _module.RegisterSubject(login, "otherEmail@mail.com", password);
        Assert.True(subject.IsFailure);
    }
}