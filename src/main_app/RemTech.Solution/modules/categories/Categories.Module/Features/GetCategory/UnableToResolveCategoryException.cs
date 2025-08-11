namespace Categories.Module.Features.GetCategory;

internal sealed class UnableToResolveCategoryException : Exception
{
    public UnableToResolveCategoryException()
        : base("Невозможно разрешить категорию.") { }

    public UnableToResolveCategoryException(Exception inner)
        : base("Невозможно разрешить категорию.", inner) { }
}
