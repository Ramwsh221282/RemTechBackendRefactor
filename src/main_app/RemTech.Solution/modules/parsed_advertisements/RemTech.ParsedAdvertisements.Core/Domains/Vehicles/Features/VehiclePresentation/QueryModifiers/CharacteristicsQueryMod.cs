using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryParameters;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class CharacteristicsQueryMod : IVehiclePresentQueryMod
{
    private readonly CharacteristicsQueryModParameter[] _parameters;
    public CharacteristicsQueryMod(CharacteristicsQueryModParameter[] parameters) =>
        _parameters = parameters;
    
    public VehiclePresentQueryStorage Modified(VehiclePresentQueryStorage storage)
    {
        if (_parameters.Length < 1)
            return storage;
        for (int i = 0; i < _parameters.Length; i++)
        {
            CharacteristicsQueryModParameter parameter = _parameters[i];
            string subQuerySql = string.Intern($"""
                                               EXISTS (
                                               SELECT 1 FROM parsed_advertisements_module.parsed_vehicle_characteristics sub_ctx
                                               WHERE sub_ctx.vehicle_id = v.id 
                                               AND sub_ctx.ctx_id = @modCtxId_{i}
                                               AND sub_ctx.ctx_name = @modCtxName_{i}
                                               AND sub_ctx.ctx_value = @modCtxValue_{i}
                                               )
                                               """);
            storage = storage.Put(subQuerySql)
                .Put($"@modCtxId_{i}", parameter.Id)
                .Put($"@modCtxName_{i}", parameter.Name)
                .Put($"@modCtxValue_{i}", parameter.Value);
        }
        return storage;
    }
}