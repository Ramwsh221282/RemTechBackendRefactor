using System.Text.Json;

namespace Timezones.Core.Models;

public static class RegionLocalDateTimeConvertersModule
{
    extension(IEnumerable<RegionLocalDateTime> dateTimes)
    {
        public string ToJson()
        {
            return JsonSerializer.Serialize(dateTimes);
        }
    }

    extension(RegionLocalDateTime)
    {
        public static IReadOnlyList<RegionLocalDateTime> ToList(string json)
        {
            return JsonSerializer.Deserialize<List<RegionLocalDateTime>>(json) ?? [];
        }
    }
}