using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailer;

public sealed record GetMailerQuery(Guid Id) : IQuery;
