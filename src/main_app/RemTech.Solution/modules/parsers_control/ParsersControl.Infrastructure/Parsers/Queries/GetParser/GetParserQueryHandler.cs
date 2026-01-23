using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParser;

public sealed class GetParserQueryHandler(ISubscribedParsersRepository repository)
	: IQueryHandler<GetParserQuery, ParserResponse?>
{
	private ISubscribedParsersRepository Repository { get; } = repository;

	public async Task<ParserResponse?> Handle(GetParserQuery query, CancellationToken ct = default)
	{
		SubscribedParserQuery spec = new SubscribedParserQuery().WithId(query.Id);
		Result<SubscribedParser> result = await Repository.Get(spec, ct);
		if (result.IsFailure)
			return null;
		return ParserResponse.Create(result.Value);
	}
}
