using System.Text.Json;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

internal static class TimeZoneRecordsStoringModule
{
    extension(TimeZoneRecord)
    {
        public static Dictionary<string, TimeZoneRecord> ToDictionary(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            Dictionary<string, TimeZoneRecord> records = [];
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {                
                TimeZoneRecord record = ParseSingleJsonToTimeZoneRecord(element);
                records[record.ZoneName] = record;
            }

            return records;
        }

        public static string ToJson(IEnumerable<TimeZoneRecord> records)
        {
            return JsonSerializer.Serialize(records);
        }

        public static Dictionary<string, TimeZoneRecord> ToDictionary(IEnumerable<TimeZoneRecord> records)
        {
            Dictionary<string, TimeZoneRecord> dict = [];
            foreach (TimeZoneRecord record in records)
            {
                dict[record.ZoneName] = record;
            }

            return dict;
        }
        
        public static IReadOnlyList<TimeZoneRecord> ToList(Dictionary<string, TimeZoneRecord> dict)
        {
            return [.. dict.Values];
        }

        private static TimeZoneRecord ParseSingleJsonToTimeZoneRecord(JsonElement element)
        {
            string name = element.GetProperty("ZoneName").GetString()!;
            ulong gmtOffset = element.GetProperty("GmtOffset").GetUInt64();
            ulong timeStamp = element.GetProperty("Timestamp").GetUInt64();
            return new TimeZoneRecord { ZoneName = name, GmtOffset = gmtOffset, Timestamp = timeStamp };
        }
    }
}
