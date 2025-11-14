using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class ChangeSubjectPasswordTests(IdentityPersistenceTestsFixture fixture)
    : IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task Change_Subject_Password_Success()
    {
        string login = "subject-login";
        string email = "subject@email.com";
        string password = "!subjectPassword!";
        string otherPassword = "!otherPassword!";

        Result<Subject> subject = await fixture.RegisterSubject(login, email, password);
        Assert.True(subject.IsSuccess);

        Result<Subject> activated = subject.Value.Activated();
        Assert.True(activated.IsSuccess);
        
        Result<Subject> updated = await fixture.UpdateSubject(activated.Value);
        Assert.True(updated.IsSuccess);
        
        Guid id = updated.Value.Snapshot().Id;
        Result<Subject> withOtherPassword = await fixture.ChangePassword(id, otherPassword);
        Assert.True(withOtherPassword.IsSuccess);

        string updatedPassword = withOtherPassword.Value.Snapshot().Password;
        Assert.Equal(updatedPassword, otherPassword);
        Assert.NotEqual(password, otherPassword);
    }
    
    [Fact]
    private async Task Change_Subject_Password_Not_Subject_Not_Exists_Failure()
    {
        string login = "subject-login";
        string email = "subject@email.com";
        string password = "!subjectPassword!";
        string otherPassword = "!otherPassword!";

        Result<Subject> subject = await fixture.RegisterSubject(login, email, password);
        Assert.True(subject.IsSuccess);
        
        Result<Subject> withOtherPassword = await fixture.ChangePassword(Guid.NewGuid(), otherPassword);
        Assert.True(withOtherPassword.IsFailure);
    }
    
    [Fact]
    private async Task Change_Subject_Password_Not_Active_Account_Failure()
    {
        string login = "subject-login";
        string email = "subject@email.com";
        string password = "!subjectPassword!";
        string otherPassword = "!otherPassword!";

        Result<Subject> subject = await fixture.RegisterSubject(login, email, password);
        Assert.True(subject.IsSuccess);
        
        Guid id = subject.Value.Snapshot().Id;
        Result<Subject> withOtherPassword = await fixture.ChangePassword(id, otherPassword);
        Assert.True(withOtherPassword.IsFailure);
    }
    
    [Fact]
    private async Task Change_Subject_Password_Not_Valid_Failure()
    {
        string login = "subject-login";
        string email = "subject@email.com";
        string password = "!subjectPassword!";
        string otherPassword = "   ";

        Result<Subject> subject = await fixture.RegisterSubject(login, email, password);
        Assert.True(subject.IsSuccess);
        
        Guid id = subject.Value.Snapshot().Id;
        Result<Subject> withOtherPassword = await fixture.ChangePassword(id, otherPassword);
        Assert.True(withOtherPassword.IsFailure);
    }
}