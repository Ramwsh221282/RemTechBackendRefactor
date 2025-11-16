using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class RequirePasswordResetTicketTests(IdentityPersistenceTestsFixture fixture) : IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task Require_Password_Reset_Ticket_Success()
    {
        string login = "testLogin";
        string email = "testEmail@mail.com";
        string password = "testPassword";
        
        Subject subject = await fixture.RegisterSubject(login, email, password);
        Guid subjectId = subject.Snapshot().Id;
        Result<SubjectTicket> result = await fixture.RequirePasswordResetTicket(subjectId);
        
        Assert.True(result.IsSuccess);
        Assert.True(await fixture.TicketIsCreatedBySubject(subjectId));
    }

    [Fact]
    private async Task Require_Password_Reset_Ticket_Failure_Subject_Not_Found()
    {
        Result<SubjectTicket> result = await fixture.RequirePasswordResetTicket(Guid.NewGuid());
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    private async Task Require_Password_Reset_Ticket_Duplicated_Ticket()
    {
        string login = "testLogin";
        string email = "testEmail@mail.com";
        string password = "testPassword";
        
        Subject subject = await fixture.RegisterSubject(login, email, password);
        Guid subjectId = subject.Snapshot().Id;
        await fixture.RequirePasswordResetTicket(subjectId);
        
        Result<SubjectTicket> result = await fixture.RequirePasswordResetTicket(subjectId);
        Assert.True(result.IsFailure);
    }
}