using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Notifications.Abstractions;
using Identity.Core.TicketsModule;

namespace Identity.Core.SubjectsModule.Notifications;

public sealed record CreatedTicket(SubjectSnapshot Subject, TicketSnapshot Ticket) : Notification;