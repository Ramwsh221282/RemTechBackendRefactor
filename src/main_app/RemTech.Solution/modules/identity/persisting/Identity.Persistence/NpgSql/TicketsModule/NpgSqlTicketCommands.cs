using Identity.Core.TicketsModule.Contracts;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.TicketsModule;

public sealed record NpgSqlTicketCommands(
    InsertTicket Insert,
    DeleteTicket Delete,
    UpdateTicket Update,
    GetTicket Get);