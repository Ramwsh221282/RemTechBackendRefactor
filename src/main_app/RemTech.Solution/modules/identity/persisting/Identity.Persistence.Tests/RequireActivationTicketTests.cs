using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.TicketsModule;
using Identity.Core.TicketsModule.Contracts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;

namespace Identity.Persistence.Tests;

public sealed class RequireActivationTicketTests(IdentityPersistenceTestsFixture fixture) : IClassFixture<IdentityPersistenceTestsFixture>
{
    [Fact]
    private async Task Require_Subject_Activation_Ticket_Success()
    {
        string email = "subject@email.com";
        string login = "subjectLogin";
        string password = "password";
        Subject subject = await fixture.RegisterSubject(login, email, password);
        Guid id = subject.Snapshot().Id;
        Result<SubjectTicket> ticket = await fixture.RequireActivationTicket(id);
        Assert.True(ticket.IsSuccess);
        Assert.True(await EnsureTicketsStorageHasTicketOfCreator(id));
    }
    
    [Fact]
    private async Task Require_Subject_Activation_Ticket_Duplicate_Failure()
    {
        string email = "subject@email.com";
        string login = "subjectLogin";
        string password = "password";
        Subject subject = await fixture.RegisterSubject(login, email, password);
        Guid id = subject.Snapshot().Id;
        await fixture.RequireActivationTicket(id);
        Result<SubjectTicket> ticket = await fixture.RequireActivationTicket(id);
        Assert.True(ticket.IsFailure);
    }
    
    [Fact]
    private async Task Require_Subject_Activation_Ticket_Subject_Not_Found()
    {
        Guid id = Guid.NewGuid();
        Result<SubjectTicket> ticket = await fixture.RequireActivationTicket(id);
        Assert.True(ticket.IsFailure);
        Assert.False(await EnsureTicketsStorageHasTicketOfCreator(id));
    }

    private async Task<bool> EnsureTicketsStorageHasTicketOfCreator(Guid Id)
    {
        await using AsyncServiceScope scope = fixture.Scope();
        TicketsStorage tickets = scope.Resolve<TicketsStorage>();
        QueryTicketArgs args = new(CreatorId: Id);
        Optional<Ticket> ticket = await tickets.Find(args, CancellationToken.None);
        return ticket.HasValue;
    }
}