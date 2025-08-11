namespace Brands.Module.Types;

internal interface IBrand
{
    Guid Id { get; }
    string Name { get; }
    long Rating { get; }
}
