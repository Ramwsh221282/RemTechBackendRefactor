namespace Categories.Module.Types;

internal interface ICategory
{
    Guid Id { get; }
    string Name { get; }
    long Rating { get; }
}
