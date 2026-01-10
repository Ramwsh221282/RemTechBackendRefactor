using Notifications.Core.PendingEmails;
using Notifications.Core.PendingEmails.Contracts;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;
public sealed record GetMailersQuery() : IQuery;