namespace Categories.Module.Features.GetCategory;

internal sealed class GetNewlyCreatedCategoryDuplicateNameException : Exception
{
    public GetNewlyCreatedCategoryDuplicateNameException(string name)
        : base($"Дубликат категории по имени {name}") { }

    public GetNewlyCreatedCategoryDuplicateNameException(string name, Exception inner)
        : base($"Дубликат категории по имени {name}", inner) { }
}
