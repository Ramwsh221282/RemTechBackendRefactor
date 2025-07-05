using Dapper.FluentMap.Mapping;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessModels;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessMappings;

public sealed class ParserDaoEntityMap : EntityMap<ParserDao>
{
    public ParserDaoEntityMap()
    {
        Map(p => p.Id).ToColumn("id");
        Map(p => p.Name).ToColumn("name");
        Map(p => p.Type).ToColumn("type");
        Map(p => p.State).ToColumn("state");
        Map(p => p.Processed).ToColumn("processed");
        Map(p => p.TotalSeconds).ToColumn("total_seconds");
        Map(p => p.Hours).ToColumn("hours");
        Map(p => p.Minutes).ToColumn("minutes");
        Map(p => p.Seconds).ToColumn("seconds");
        Map(p => p.WaitDays).ToColumn("wait_days");
        Map(p => p.NextRun).ToColumn("next_run");
        Map(p => p.LastRun).ToColumn("last_run");
    }
}
