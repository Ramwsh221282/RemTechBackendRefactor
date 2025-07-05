using System.Data.Common;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserStatisticFromSqlRow(DbDataReader reader)
{
    public ParserStatistic Read() =>
        new(
            new WorkingStatistic(
                new WorkingTime(
                    PositiveLong.New(reader.GetInt64(reader.GetOrdinal("total_seconds"))),
                    new Hour(PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("hours")))),
                    new Minutes(PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("minutes")))),
                    new Seconds(PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("seconds"))))
                ),
                new IncrementableNumber(
                    PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("processed")))
                )
            )
        );
}
