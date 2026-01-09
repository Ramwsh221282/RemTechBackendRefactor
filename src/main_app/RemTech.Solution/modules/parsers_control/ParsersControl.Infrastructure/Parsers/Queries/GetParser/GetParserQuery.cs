using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParser;

public record GetParserQuery(Guid Id) : IQuery;