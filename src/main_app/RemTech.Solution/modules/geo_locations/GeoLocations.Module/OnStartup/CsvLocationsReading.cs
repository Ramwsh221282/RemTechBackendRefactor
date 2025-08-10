using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using GeoLocations.Module.Features.Persisting;

namespace GeoLocations.Module.OnStartup;

internal sealed class CsvLocationsReading
{
    private static readonly string Path = System.IO.Path.Combine(
        Environment.CurrentDirectory,
        "OnStartup",
        "city.csv"
    );
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;

    public IEnumerable<RegionToPersist> Read()
    {
        if (!File.Exists(Path))
            throw new ApplicationException("Unable to find city file");
        Dictionary<string, RegionToPersist> regions = [];
        CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
        using StreamReader reader = new StreamReader(Path);
        using CsvReader csv = new CsvReader(reader, configuration);
        while (csv.Read())
        {
            RegionCsvRecord record = csv.GetRecord<RegionCsvRecord>();
            if (!regions.TryGetValue(record.region, out RegionToPersist? region))
            {
                region = new RegionToPersist(Guid.NewGuid(), record.region, record.region_type);
                regions.Add(record.region, region);
            }

            region = region.WithCity(record.city);
            regions[record.region] = region;
        }

        return regions.Values.AsEnumerable();
    }
}
