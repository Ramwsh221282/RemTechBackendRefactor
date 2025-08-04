namespace RemTech.Vehicles.Module.Types.Brands.Decorators.Postgres;

internal sealed class UnableToStoreBrandException : Exception
{
    public UnableToStoreBrandException(string message)
        : base(message) { }
}
