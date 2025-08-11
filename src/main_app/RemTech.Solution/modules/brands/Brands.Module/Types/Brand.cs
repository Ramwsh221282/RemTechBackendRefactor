namespace Brands.Module.Types;

internal sealed record Brand(Guid Id, string Name, long Rating) : IBrand { }
