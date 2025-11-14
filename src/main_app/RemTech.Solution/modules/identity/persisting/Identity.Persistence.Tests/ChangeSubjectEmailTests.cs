using Identity.Core.SubjectsModule.Domain.Subjects;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class ChangeSubjectEmailTests(IdentityPersistenceTestsFixture fixture) 
: IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task Change_Subject_Email_Success()
    {
        string subjectLogin = "subject_login";
        string subjectPassword = "!myPassword123!";
        string subjectEmail = "subject@email.com";
        string otherSubjectEmail = "otherfffEmail@mail.com";
        
        Result<Subject> subject = await fixture.RegisterSubject(subjectLogin, subjectEmail, subjectPassword);
        Assert.True(subject.IsSuccess);

        Result<Subject> activated = subject.Value.Activated();
        Assert.True(activated.IsSuccess);
        
        Result<Subject> updated = await fixture.UpdateSubject(activated);
        Assert.True(updated.IsSuccess);
        
        Guid id = updated.Value.Snapshot().Id;
        Result<Subject> withOtherEmail = await fixture.ChangeSubjectEmail(id, otherSubjectEmail);
        Assert.True(withOtherEmail.IsSuccess);
    }
    
    [Fact]
    private async Task Change_Subject_Email_Not_Active_Account_Failure()
    {
        string subjectLogin = "subject_login";
        string subjectPassword = "!myPassword123!";
        string subjectEmail = "subject@email.com";
        string otherSubjectEmail = "otherfffEmail@mail.com";
        Result<Subject> subject = await fixture.RegisterSubject(subjectLogin, subjectEmail, subjectPassword);
        Assert.True(subject.IsSuccess);
        Guid id = subject.Value.Snapshot().Id;
        Result<Subject> withOtherEmail = await fixture.ChangeSubjectEmail(id, otherSubjectEmail);
        Assert.True(withOtherEmail.IsFailure);
    }

    [Fact]
    private async Task Change_Subject_Invalid_Email_Failure()
    {
        string subjectLogin = "subject_login";
        string subjectPassword = "!myPassword123!";
        string subjectEmail = "subject@email.com";
        string otherSubjectEmail = "not really an email";
        Result<Subject> subject = await fixture.RegisterSubject(subjectLogin, subjectEmail, subjectPassword);
        Assert.True(subject.IsSuccess);
        Guid id = subject.Value.Snapshot().Id;
        Result<Subject> withOtherEmail = await fixture.ChangeSubjectEmail(id, otherSubjectEmail);
        Assert.True(withOtherEmail.IsFailure);
    }
    
    [Fact]
    private async Task Not_Existing_Subject_Changes_Email_Failure()
    {
        string subjectEmail = "subject@email.com";
        Result<Subject> withOtherEmail = await fixture.ChangeSubjectEmail(Guid.NewGuid(), subjectEmail);
        Assert.True(withOtherEmail.IsFailure);
    }
}