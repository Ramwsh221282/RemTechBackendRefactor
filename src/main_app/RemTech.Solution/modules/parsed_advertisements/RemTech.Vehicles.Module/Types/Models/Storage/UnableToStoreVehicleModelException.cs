namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class UnableToStoreVehicleModelException : Exception
{
    public UnableToStoreVehicleModelException(string message)
        : base(message) { }
}
