namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryByNameNotFoundException : Exception
{
    public GetCategoryByNameNotFoundException(string name)
        : base($"Категория {name} не найдена.") { }
}
