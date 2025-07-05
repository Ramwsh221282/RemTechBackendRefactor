namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.SelectConstants;

public sealed class SelectParserWithLeftJoinedLinks
{
    private const string _sqlPart = """
        p.id,
        p.name,
        p.type,
        p.state,
        p.domain,
        p.processed,
        p.total_seconds,
        p.hours,
        p.minutes,
        p.seconds,
        p.wait_days,
        p.next_run,
        p.last_run,
        pl.id as link_id,
        pl.parser_id,
        pl.name as link_name,
        pl.url,
        pl.activity,
        pl.processed as link_processed,
        pl.total_seconds as link_total_seconds,
        pl.hours as link_hours,
        pl.minutes as link_minutes,
        pl.seconds as link_seconds
        """;

    public string Read() => _sqlPart;
}
