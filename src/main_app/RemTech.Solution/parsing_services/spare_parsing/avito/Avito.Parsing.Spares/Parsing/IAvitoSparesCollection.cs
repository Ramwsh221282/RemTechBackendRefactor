namespace Avito.Vehicles.Service.Parsing;

public interface IAvitoSparesCollection
{
    Task<IEnumerable<AvitoSpare>> Read();
}
