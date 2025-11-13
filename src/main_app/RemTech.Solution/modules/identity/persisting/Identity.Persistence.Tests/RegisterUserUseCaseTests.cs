using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class RegisterUserUseCaseTests(IdentityPersistenceTestsFixture fixture) : IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task Register_User_With_Persisting_Success()
    {
        string email = "myEmail@gmail.com";
        string password = "myPassword";
        string login = "myLogin";
        CancellationToken ct = CancellationToken.None;

        await using (AsyncServiceScope scope = fixture.Scope())
        {
            RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
            RegisterSubject useCase = scope.ServiceProvider.Resolve<RegisterSubject>();
            Result<Subject> subject = await useCase(args);
            Assert.True(subject.IsSuccess);
        }
    }

    [Fact]
    private async Task Register_User_Duplicate_Email_Failure()
    {
        string email = "myEmail@gmail.com";
        string password = "myPassword";
        string login = "myLogin";
        CancellationToken ct = CancellationToken.None;

        await using (AsyncServiceScope scope = fixture.Scope())
        {
            RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
            RegisterSubject useCase = scope.ServiceProvider.Resolve<RegisterSubject>();
            Result<Subject> subject = await useCase(args);
            Assert.True(subject.IsSuccess);
        }
        
        await using (AsyncServiceScope scope = fixture.Scope())
        {
            RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
            args = args with { Login = "otherLogin" };
            RegisterSubject useCase = scope.ServiceProvider.Resolve<RegisterSubject>();
            Result<Subject> subject = await useCase(args);
            Assert.True(subject.IsFailure);
        }
    }
    
    [Fact]
    private async Task Register_User_Duplicate_Login_Failure()
    {
        string email = "myEmail@gmail.com";
        string password = "myPassword";
        string login = "myLogin";
        CancellationToken ct = CancellationToken.None;

        await using (AsyncServiceScope scope = fixture.Scope())
        {
            RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
            RegisterSubject useCase = scope.ServiceProvider.Resolve<RegisterSubject>();
            Result<Subject> subject = await useCase(args);
            Assert.True(subject.IsSuccess);
        }
        
        await using (AsyncServiceScope scope = fixture.Scope())
        {
            RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
            args = args with { Email = "otherEmail@mail.com" };
            RegisterSubject useCase = scope.ServiceProvider.Resolve<RegisterSubject>();
            Result<Subject> subject = await useCase(args);
            Assert.True(subject.IsFailure);
        }
    }
}