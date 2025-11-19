using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using RemTech.BuildingBlocks.DependencyInjection;
using Tests.ModuleFixtures;
using Tickets.EventListeners;

namespace Tests.Identity;

public sealed class RequireActivationTicketTests(CompositionRootFixture fixture) : IClassFixture<CompositionRootFixture>
{
    private readonly IdentityModule _module = fixture.IdentityModule;
    private readonly TicketsModule _tickets = fixture.TicketsModule;

    [Fact]
    private async Task Require_Subject_Activation_Ticket_Success()
    {
        await using AsyncServiceScope scope = fixture.Scope();
        TicketCreatedEventListener listener = scope.Resolve<TicketCreatedEventListener>();
        await listener.StartAsync(CancellationToken.None);
        
        string email = "subject@email.com";
        string login = "subjectLogin";
        string password = "password";
        Subject subject = await _module.RegisterSubject(login, email, password);
        Guid id = subject.Snapshot().Id;
        Result<SubjectTicket> ticket = await _module.RequireActivationTicket(id);
        Assert.True(ticket.IsSuccess);
        bool has = await _tickets.HasTickets(30);
        Assert.True(has);
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
        // Assert.False(await _module.TicketIsCreatedBySubject(id));
    }
}