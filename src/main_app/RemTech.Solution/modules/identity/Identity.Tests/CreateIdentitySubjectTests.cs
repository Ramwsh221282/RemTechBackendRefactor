using Identity.Application.Features.SubjectRegistration;
using Identity.Core.SubjectsModule.Contexts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Persistence.NpgSql.SubjectsModule;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Tests;

public sealed class CreateIdentitySubjectTests(IdentityTestsService service) : IClassFixture<IdentityTestsService>
{
    private async Task<Subject> Seeded(string login, string email, string password)
    {
        Subject subject = Subject.Create(email, login, password);
        await using AsyncServiceScope scope = service.Scope();
        NpgSqlSubjectCommands commands = scope.ServiceProvider.Resolve<NpgSqlSubjectCommands>();
        await commands.Insert(subject, CancellationToken.None);
        return subject;
    }
    
    [Fact]
    private async Task Find_Subject_TDD_Success()
    {
        
    }
    
    [Fact]
    private async Task Create_Identity_Subject_Success()
    {
        string email = "someBigEmail@mail.com";
        string password = "SeriousPassword123";
        string login = "SeriousLogin";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterSubject registration = scope.ServiceProvider.GetRequiredService<RegisterSubject>();
            return await registration.Register(args);
        });
        
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    private async Task Create_Identity_Subject_Failure_Email()
    {
        string email = "some random string";
        string password = "SeriousPassword123";
        string login = "SeriousLogin";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterSubject registration = scope.ServiceProvider.GetRequiredService<RegisterSubject>();
            return await registration.Register(args);
        });
        
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Create_Identity_Subject_Failure_Password()
    {
        string email = "someBigEmail@mail.com";
        string password = "  ";
        string login = "SeriousLogin";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterSubject registration = scope.ServiceProvider.GetRequiredService<RegisterSubject>();
            return await registration.Register(args);
        });
        
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Create_Identity_Subject_Failure_Login()
    {
        string email = "someBigEmail@mail.com";
        string password = "SeriousPassword123";
        string login = "   ";
        CancellationToken ct = CancellationToken.None;
        RegisterSubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterSubject registration = scope.ServiceProvider.GetRequiredService<RegisterSubject>();
            return await registration.Register(args);
        });
        
        Assert.True(result.IsFailure);
    }
}