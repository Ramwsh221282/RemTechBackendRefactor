using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;

public sealed class VehiclePresentQueryStorage
{
    private readonly List<string> _mods = [];
    private readonly ParametrizingPgCommand _command;

    public VehiclePresentQueryStorage(ParametrizingPgCommand command) =>
        _command = command;

    private VehiclePresentQueryStorage(VehiclePresentQueryStorage origin, ParametrizingPgCommand redacted)
    {
        _mods = origin._mods;
        _command = redacted;
    }

    private VehiclePresentQueryStorage(VehiclePresentQueryStorage origin)
    {
        _mods = origin._mods;
        _command = origin._command;
    }
    
    public VehiclePresentQueryStorage Put<T>(string modification, string parameter, T argument)
    {
        _mods.Add(modification);
        ParametrizingPgCommand redacted = _command.With(parameter, argument);
        return new VehiclePresentQueryStorage(this, redacted);
    }

    public VehiclePresentQueryStorage Put<T>(string parameter, T argument)
    {
        ParametrizingPgCommand redacted = _command.With(parameter, argument);
        return new VehiclePresentQueryStorage(this, redacted);
    }

    public VehiclePresentQueryStorage Put(string modification)
    {
        _mods.Add(modification);
        return new VehiclePresentQueryStorage(this);
    }

    public SqlRedactingPgCommand Modified()
    {
        string modsString = string.Join(" AND ", _mods);
        string origin = _command.Command().CommandText;
        string sql = string.Format(origin, modsString);
        return new SqlRedactingPgCommand(_command).Redacted(sql);
    }

    public string Sql()
    {
        string modsString = string.Join(" AND ", _mods);
        string origin = _command.Command().CommandText;
        string sql = string.Format(origin, modsString);
        return sql;
    }
}