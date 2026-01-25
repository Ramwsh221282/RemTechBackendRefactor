using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766859347)]
public sealed class WorkingParserStartDateEndDateColumns : Migration
{
    public override void Up()
    {
        Alter.Table("working_parsers").InSchema("drom_vehicles_parser").AddColumn("start_date_time").AsDateTime().Nullable();
        Alter.Table("working_parsers").InSchema("drom_vehicles_parser").AddColumn("end_date_time").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Column("start_date_time").FromTable("working_parsers").InSchema("drom_vehicles_parser");
        Delete.Column("end_date_time").FromTable("working_parsers").InSchema("drom_vehicles_parser");
    }
}