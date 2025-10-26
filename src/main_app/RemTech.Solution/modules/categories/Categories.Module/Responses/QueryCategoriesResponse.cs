namespace Categories.Module.Responses;

public sealed record QueryCategoriesResponse(long Count, IEnumerable<CategoryDto> Categories);
