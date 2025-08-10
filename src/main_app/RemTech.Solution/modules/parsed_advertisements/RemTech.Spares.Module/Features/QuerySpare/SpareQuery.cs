namespace RemTech.Spares.Module.Features.QuerySpare;

public sealed record SpareQuery(SparePagination Pagination, SpareTextSearch? Text = null);
