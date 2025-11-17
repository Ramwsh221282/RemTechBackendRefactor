using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Domain.Tickets;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Notifications;

public sealed record CreatedTicket(SubjectSnapshot Subject, SubjectTicketSnapshot Ticket) : Notification;