using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.UseCases.Tests;

public sealed class RegisterSubjectUseCaseTests
{
    [Fact]
    private async Task Register_Subject_Success()
    {
        string email = "myEmail@mail.com";
        string login = "myLogin";
        string password = "myPassword";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
        RegisterSubject useCase = GetUseCase();
        Result<Subject> result = await useCase(args);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    private async Task Register_Subject_Empty_Email_Failure()
    {
        string email = " ";
        string login = "myLogin";
        string password = "myPassword";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
        RegisterSubject useCase = GetUseCase();
        Result<Subject> result = await useCase(args);
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Register_Subject_Invalid_Email_Format_Failure()
    {
        string email = "not valid email format";
        string login = "myLogin";
        string password = "myPassword";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
        RegisterSubject useCase = GetUseCase();
        Result<Subject> result = await useCase(args);
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Register_Subject_Empty_Login_Failure()
    {
        string email = "not valid email format";
        string login = "  ";
        string password = "myPassword";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
        RegisterSubject useCase = GetUseCase();
        Result<Subject> result = await useCase(args);
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Register_Subject_Empty_Password_Failure()
    {
        string email = "not valid email format";
        string login = "  ";
        string password = "myPassword";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectUseCaseArgs args = new(login, email, password, ct);
        RegisterSubject useCase = GetUseCase();
        Result<Subject> result = await useCase(args);
        Assert.True(result.IsFailure);
    }

    private RegisterSubject GetUseCase()
    {
        return RegisterSubjectUseCase.RegisterSubject();
    }
}