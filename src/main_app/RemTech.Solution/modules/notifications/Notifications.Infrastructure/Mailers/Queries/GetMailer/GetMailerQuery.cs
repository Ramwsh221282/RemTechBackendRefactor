using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailer;

/// <summary>
/// Запрос на получение почтового ящика по идентификатору.
/// </summary>
/// <param name="Id">Идентификатор почтового ящика.</param>
public sealed record GetMailerQuery(Guid Id) : IQuery;
