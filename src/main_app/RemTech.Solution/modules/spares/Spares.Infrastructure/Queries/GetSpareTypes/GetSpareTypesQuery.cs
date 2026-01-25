using RemTech.SharedKernel.Core.Handlers;

namespace Spares.Infrastructure.Queries.GetSpareTypes;

public sealed class GetSpareTypesQuery : IQuery
{
	private GetSpareTypesQuery() { }

	public string? TextSearch { get; private init; }
	public int? Amount { get; private init; }

	public GetSpareTypesQuery WithTextSearch(string? text) => Clone(this, textSearch: text);

	public GetSpareTypesQuery WithAmount(int? amount) => Clone(this, amount: amount);

	public static GetSpareTypesQuery Create() => new();

	private static GetSpareTypesQuery Clone(GetSpareTypesQuery origin, string? textSearch = null, int? amount = null) =>
		new()
		{
			TextSearch = string.IsNullOrWhiteSpace(textSearch) ? origin.TextSearch : textSearch,
			Amount = amount ?? origin.Amount,
		};
}
