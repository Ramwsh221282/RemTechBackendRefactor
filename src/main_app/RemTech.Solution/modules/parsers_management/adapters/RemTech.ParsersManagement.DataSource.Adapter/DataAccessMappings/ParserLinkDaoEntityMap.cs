using Dapper.FluentMap.Mapping;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessModels;

namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessMappings;

public sealed class ParserLinkDaoEntityMap : EntityMap<ParserLinkDao>
{
    public ParserLinkDaoEntityMap()
    {
        Map(l => l.Id).ToColumn("id");
        Map(l => l.ParserId).ToColumn("parser_id");
        Map(l => l.Name).ToColumn("name");
        Map(l => l.Url).ToColumn("url");
        Map(l => l.Activity).ToColumn("activity");
        Map(l => l.Processed).ToColumn("processed");
        Map(l => l.TotalSeconds).ToColumn("total_seconds");
        Map(l => l.Hours).ToColumn("hours");
        Map(l => l.Minutes).ToColumn("minutes");
        Map(l => l.Seconds).ToColumn("seconds");
    }
}
