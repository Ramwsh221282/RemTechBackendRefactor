using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

/// <summary>
/// Запрос на получение множества почтовых ящиков.
/// </summary>
public sealed record GetMailersQuery() : IQuery;
