using FluentMigrator;

namespace AvitoSparesParser.Database;

[Migration(1766848275)]
public sealed class SchemaMigrations : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE schema if not exists avito_spares_parser");
    }

    public override void Down()
    {
        Delete.Schema("avito_spares_parser");
    }
}