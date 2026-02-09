using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParser;

/// <summary>
///     Запрос на получение парсера по его идентификатору.
/// </summary>
/// <param name="Id">Идентификатор парсера.</param>
public record GetParserQuery(Guid Id) : IQuery;
