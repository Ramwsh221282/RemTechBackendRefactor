using Identity.Application.Features;
using Identity.Core.SubjectsModule.Models;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Tests;

public sealed class CreateIdentitySubjectTests(IdentityTestsService service) : IClassFixture<IdentityTestsService>
{
    [Fact]
    private async Task Create_Identity_Subject_Success()
    {
        string email = "someBigEmail@mail.com";
        string password = "SeriousPassword123";
        string login = "SeriousLogin";
        CancellationToken ct = CancellationToken.None;
        RegisterIdentitySubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterIdentitySubject registration = scope.ServiceProvider.GetRequiredService<RegisterIdentitySubject>();
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
        RegisterIdentitySubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterIdentitySubject registration = scope.ServiceProvider.GetRequiredService<RegisterIdentitySubject>();
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
        RegisterIdentitySubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterIdentitySubject registration = scope.ServiceProvider.GetRequiredService<RegisterIdentitySubject>();
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
        RegisterIdentitySubjectArgs args = new(email, password, login, ct);

        Result<Subject> result = await service.Sp.ExecuteInScopeAsync(async scope =>
        {
            RegisterIdentitySubject registration = scope.ServiceProvider.GetRequiredService<RegisterIdentitySubject>();
            return await registration.Register(args);
        });
        
        Assert.True(result.IsFailure);
    }
}