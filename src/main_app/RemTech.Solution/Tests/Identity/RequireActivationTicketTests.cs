using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Tests.ModuleFixtures;

namespace Tests.Identity;

public sealed class RequireActivationTicketTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly IdentityModule _module = fixture.IdentityModule;

    [Fact]
    private async Task Require_Subject_Activation_Ticket_Success()
    {
        string email = "subject@email.com";
        string login = "subjectLogin";
        string password = "password";
        Subject subject = await _module.RegisterSubject(login, email, password);
        Guid id = subject.Snapshot().Id;
        Result<SubjectTicket> ticket = await _module.RequireActivationTicket(id);
        Assert.True(ticket.IsSuccess);
        Assert.True(await _module.TicketIsCreatedBySubject(id));
    }

    [Fact]
    private async Task Require_Subject_Activation_Ticket_Duplicate_Failure()
    {
        string email = "subject@email.com";
        string login = "subjectLogin";
        string password = "password";
        Subject subject = await _module.RegisterSubject(login, email, password);
        Guid id = subject.Snapshot().Id;
        await _module.RequireActivationTicket(id);
        Result<SubjectTicket> ticket = await _module.RequireActivationTicket(id);
        Assert.True(ticket.IsFailure);
    }

    [Fact]
    private async Task Require_Subject_Activation_Ticket_Subject_Not_Found()
    {
        Guid id = Guid.NewGuid();
        Result<SubjectTicket> ticket = await _module.RequireActivationTicket(id);
        Assert.True(ticket.IsFailure);
        Assert.False(await _module.TicketIsCreatedBySubject(id));
    }
}