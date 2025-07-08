using System.Data.Common;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserLinkStatisticFromSqlRow(DbDataReader reader)
{
    public ParserLinkStatistic Read() =>
        new(
            new WorkingStatistic(
                new WorkingTime(
                    PositiveLong.New(reader.GetInt64(reader.GetOrdinal("link_total_seconds"))),
                    new Hour(PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("link_hours")))),
                    new Minutes(
                        PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("link_minutes")))
                    ),
                    new Seconds(
                        PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("link_seconds")))
                    )
                ),
                new IncrementableNumber(
                    PositiveInteger.New(reader.GetInt32(reader.GetOrdinal("link_processed")))
                )
            )
        );
}
