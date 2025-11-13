using Identity.Persistence.NpgSql.SubjectsModule;
using Identity.Persistence.NpgSql.TicketsModule;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql;

public sealed record NpgSqlIdentityCommands(
    NpgSqlSession Session,
    NpgSqlSubjectCommands Subjects,
    NpgSqlTicketCommands Tickets) : IDisposable, IAsyncDisposable
{
    public void Dispose()
    {
        Session.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Session.DisposeAsync();
    }
}