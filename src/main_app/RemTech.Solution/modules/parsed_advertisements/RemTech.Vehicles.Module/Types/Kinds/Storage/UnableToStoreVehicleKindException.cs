namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class UnableToStoreVehicleKindException : Exception
{
    public UnableToStoreVehicleKindException(string message)
        : base(message) { }
}
