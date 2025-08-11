namespace Categories.Module.Features.GetCategory;

internal sealed class GetCategoryByNameNameEmptyException : Exception
{
    public GetCategoryByNameNameEmptyException()
        : base("Невозможно получить категорию по имени. Название пустое.") { }
}
