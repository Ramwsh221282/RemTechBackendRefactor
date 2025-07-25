using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers;

public sealed class ParsersDatabaseBakery
{
    private readonly DatabaseBakery _bakery;

    public ParsersDatabaseBakery(DatabaseConfiguration configuration)
    {
        _bakery = new DatabaseBakery(configuration);
    }

    public void Up()
    {
        _bakery.Up(typeof(ParsersDatabaseBakery).Assembly);
    }

    public Task Down()
    {
        return _bakery.Down(
            "parsers_management_module.contained_items",
            "parsers_management_module.parser_links",
            "parsers_management_module.parsers"
        );
    }
}
