namespace RemTech.Vehicles.Module.Types.Kinds.Storage;

internal sealed class UnableToStoreVehicleKindException : Exception
{
    public UnableToStoreVehicleKindException(string message, string kind)
        : base($"{message} {kind}") { }

    public UnableToStoreVehicleKindException(string message, string kind, Exception ex)
        : base($"{message} {kind}", ex) { }
}
