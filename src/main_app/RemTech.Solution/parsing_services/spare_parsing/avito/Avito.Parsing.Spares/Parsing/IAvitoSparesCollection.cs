namespace Avito.Parsing.Spares.Parsing;

public interface IAvitoSparesCollection
{
    Task<IEnumerable<AvitoSpare>> Read();
}
