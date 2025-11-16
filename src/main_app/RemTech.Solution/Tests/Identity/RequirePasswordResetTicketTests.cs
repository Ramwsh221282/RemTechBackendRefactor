using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Tests.ModuleFixtures;

namespace Tests.Identity;

public sealed class RequirePasswordResetTicketTests(CompositionRootFixture fixture)
    : IClassFixture<CompositionRootFixture>
{
    private readonly IdentityModule _module = fixture.IdentityModule;

    [Fact]
    private async Task Require_Password_Reset_Ticket_Success()
    {
        string login = "testLogin";
        string email = "testEmail@mail.com";
        string password = "testPassword";

        Subject subject = await _module.RegisterSubject(login, email, password);
        Guid subjectId = subject.Snapshot().Id;
        Result<SubjectTicket> result = await _module.RequirePasswordResetTicket(subjectId);

        Assert.True(result.IsSuccess);
        Assert.True(await _module.TicketIsCreatedBySubject(subjectId));
    }

    [Fact]
    private async Task Require_Password_Reset_Ticket_Failure_Subject_Not_Found()
    {
        Result<SubjectTicket> result = await _module.RequirePasswordResetTicket(Guid.NewGuid());
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Require_Password_Reset_Ticket_Duplicated_Ticket()
    {
        string login = "testLogin";
        string email = "testEmail@mail.com";
        string password = "testPassword";

        Subject subject = await _module.RegisterSubject(login, email, password);
        Guid subjectId = subject.Snapshot().Id;
        await _module.RequirePasswordResetTicket(subjectId);

        Result<SubjectTicket> result = await _module.RequirePasswordResetTicket(subjectId);
        Assert.True(result.IsFailure);
    }
}