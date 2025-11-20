using Identity.Core.SubjectsModule.Contracts;
using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Tests.ModuleFixtures;
using Tickets.Core;

namespace Tests.Identity.Features;

public sealed class RequireActivationTicketTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly IdentityModule _module = fixture.IdentityModule;
    private readonly TicketsModule _tickets = fixture.TicketsModule;

    [Fact]
    private async Task Require_Subject_Activation_Ticket_Success()
    {
        await using AsyncServiceScope scope = fixture.Scope();
        string email = "subject@email.com";
        string login = "subjectLogin";
        string password = "password";
        Subject subject = await _module.RegisterSubject(login, email, password);
        Guid id = subject.Snapshot().Id;
        Result<RequireActivationTicketResult> result = await _module.RequireActivationTicket(id);
        Assert.True(result.IsSuccess);
        await Task.Delay(TimeSpan.FromSeconds(30));
        bool has = await _tickets.HasTickets();
        IEnumerable<Ticket> tickets = await _tickets.GetTickets();
        Assert.True(has);
        Assert.True(tickets.Count() == 1);
        // TODO send email message to some email.
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
        Result<RequireActivationTicketResult> result = await _module.RequireActivationTicket(id);
        Assert.True(result.IsFailure);
    }

    [Fact]
    private async Task Require_Subject_Activation_Ticket_Subject_Not_Found()
    {
        Guid id = Guid.NewGuid();
        Result<RequireActivationTicketResult> result = await _module.RequireActivationTicket(id);
        Assert.True(result.IsFailure);
    }
}