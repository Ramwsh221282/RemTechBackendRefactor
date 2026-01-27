using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

/// <summary>
///    Запрос на получение всех парсеров.
/// </summary>
public sealed record GetParsersQuery() : IQuery;
