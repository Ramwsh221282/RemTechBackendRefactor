namespace Categories.Module.Types;

internal sealed record Category(Guid Id, string Name, long Rating) : ICategory;
