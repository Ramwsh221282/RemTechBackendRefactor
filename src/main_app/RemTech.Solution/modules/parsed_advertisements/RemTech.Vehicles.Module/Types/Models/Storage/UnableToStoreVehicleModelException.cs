namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class UnableToStoreVehicleModelException : Exception
{
    public UnableToStoreVehicleModelException(string message, string model)
        : base($"{message} {model}") { }

    public UnableToStoreVehicleModelException(string message, string model, Exception inner)
        : base($"{message} {model}", inner) { }
}
