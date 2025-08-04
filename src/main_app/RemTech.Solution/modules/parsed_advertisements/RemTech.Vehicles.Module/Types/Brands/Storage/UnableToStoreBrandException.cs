namespace RemTech.Vehicles.Module.Types.Brands.Storage;

internal sealed class UnableToStoreBrandException : Exception
{
    public UnableToStoreBrandException(string message)
        : base(message) { }
}
