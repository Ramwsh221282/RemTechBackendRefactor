namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

public sealed class PgVehicleCharacteristic
{
    private readonly Guid ctx_id;
    private readonly string ctx_name;
    private readonly string ctx_value;
    private readonly string ctx_measure;

    public PgVehicleCharacteristic(Guid ctxId, string ctxName, string ctxValue, string ctxMeasure)
    {
        ctx_id = ctxId;
        ctx_name = ctxName;
        ctx_value = ctxValue;
        ctx_measure = ctxMeasure;
    }
}
