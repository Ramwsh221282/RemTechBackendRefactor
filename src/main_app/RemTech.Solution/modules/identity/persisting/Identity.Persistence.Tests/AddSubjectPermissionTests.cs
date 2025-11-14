using Identity.Core.PermissionsModule;
using Identity.Core.PermissionsModule.Contracts;
using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class AddSubjectPermissionTests(IdentityPersistenceTestsFixture fixture) 
    : IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task AddSubject_Permission_Success()
    {
        string email = "myEmail@mail.com";
        string login = "myLogin";
        string password = "myPassword";
        string permissionName = "watches.catalogue";
        
        Subject subject = await RegisterSubject(login, email, password);
        Permission permission = await RegisterPermission(permissionName);
        
        Result<Subject> withPermission = 
            await AddSubjectPermission(subject.Snapshot().Id, permission.Snapshot().Id);
        
        Assert.True(withPermission.IsSuccess);
        
        Guid subjectId = subject.Snapshot().Id;
        Guid permissionId = permission.Snapshot().Id;
        Assert.True(await SubjectHasPermission(subjectId, permissionId));
    }

    [Fact]
    private async Task Add_Subject_Permission_Duplicate_Failure()
    {
        string email = "myEmail@mail.com";
        string login = "myLogin";
        string password = "myPassword";
        string permissionName = "watches.catalogue";
        
        Subject subject = await RegisterSubject(login, email, password);
        Permission permission = await RegisterPermission(permissionName);
        
        Result<Subject> withPermission = 
            await AddSubjectPermission(subject.Snapshot().Id, permission.Snapshot().Id);
        
        Result<Subject> withSamePermission =
            await AddSubjectPermission(subject.Snapshot().Id, permission.Snapshot().Id);
        
        Assert.True(withPermission.IsSuccess);
        Assert.True(withSamePermission.IsFailure);
    }

    [Fact]
    private async Task Add_Subject_Permission_Not_Existed_Subject()
    {
        string permissionName = "watches.catalogue";
        Permission permission = await RegisterPermission(permissionName);
        Result<Subject> result = await AddSubjectPermission(Guid.NewGuid(), permission.Snapshot().Id);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    private async Task Add_Subject_Permission_Not_Existing_Permission()
    {
        
        string email = "myEmail@mail.com";
        string login = "myLogin";
        string password = "myPassword"; 
        Subject subject = await RegisterSubject(login, email, password);
        Result<Subject> result = await AddSubjectPermission(subject.Snapshot().Id, Guid.NewGuid());
        Assert.True(result.IsFailure);
    }
    
    private async Task<Result<Subject>> AddSubjectPermission(Guid subjectId, Guid permissionId)
    {
        AddSubjectPermissionArgs args = new(
            permissionId, 
            subjectId, 
            Optional.None<Subject>(), 
            Optional.None<Permission>(),
            CancellationToken.None);
        await using AsyncServiceScope scope = fixture.Scope();
        AddSubjectPermission useCase = scope.Resolve<AddSubjectPermission>();
        return await useCase(args);
    }

    private async Task<bool> SubjectHasPermission(Guid subjectId, Guid permissionId)
    {
        await using AsyncServiceScope scope = fixture.Scope();
        SubjectQueryArgs args = new(Id: subjectId);
        SubjectsStorage storage = scope.Resolve<SubjectsStorage>();
        Optional<Subject> subject = await storage.Find(args, CancellationToken.None);
        if (subject.NoValue) return false;
        SubjectSnapshot snap = subject.Value.Snapshot();
        return snap.Permissions.Any(p => p.Id == permissionId);
    }
    
    private async Task<Permission> RegisterPermission(string name)
    {
        RegisterPermissionArgs args = new(name, CancellationToken.None);
        await using AsyncServiceScope scope = fixture.Scope();
        RegisterPermission usecase = scope.Resolve<RegisterPermission>();
        return await usecase(args);
    }
    
    private async Task<Subject> RegisterSubject(string login, string email, string password)
    {
        RegisterSubjectUseCaseArgs args = new(login, email, password, CancellationToken.None);
        await using AsyncServiceScope scope = fixture.Scope();
        RegisterSubject usecase = scope.Resolve<RegisterSubject>();
        return await usecase(args);
    }
}