namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class UnableToStoreBrandException : Exception
{
    public UnableToStoreBrandException(string message, string brandName)
        : base($"{message} {brandName}") { }

    public UnableToStoreBrandException(string message, string brandName, Exception inner)
        : base($"{message} {brandName}", inner) { }
}
