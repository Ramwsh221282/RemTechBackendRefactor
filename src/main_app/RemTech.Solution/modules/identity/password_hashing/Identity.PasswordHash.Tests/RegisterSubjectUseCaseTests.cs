using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.PasswordHash.Tests;

public sealed class RegisterSubjectUseCaseTests(PasswordHashTestsFixture fixture) : IClassFixture<PasswordHashTestsFixture>
{
    [Fact]
    private async Task Register_Use_Password_Ensure_Hashed()
    {
        string email = "myEmail@mail.com";
        string password = "mySuperPassword";
        string login = "superLogin";
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = fixture.Scope();
        RegisterSubject registration = scope.ServiceProvider.Resolve<RegisterSubject>();
        RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
        Result<Subject> registered = await registration(args);
        Assert.True(registered.IsSuccess);
        string registeredPassword = registered.Value.Snapshot().Password;
        Assert.NotEqual(password, registeredPassword);
    }
    
    
}