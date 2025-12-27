using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[Migration(1766852546)]
public sealed class ParsersAddTimeTrackingColumns : Migration
{
    public override void Up()
    {
        Alter.Table("parsers").InSchema("avito_parser_module").AddColumn("start_datetime").AsDateTime().Nullable();
        Alter.Table("parsers").InSchema("avito_parser_module").AddColumn("end_datetime").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Column("start_datetime").FromTable("parsers").InSchema("avito_parser_module");
        Delete.Column("end_datetime").FromTable("parsers").InSchema("avito_parser_module");
    }
}