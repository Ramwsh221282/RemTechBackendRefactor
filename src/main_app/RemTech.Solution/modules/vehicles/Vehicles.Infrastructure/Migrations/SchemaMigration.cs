using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767027778)]
public sealed class SchemaMigration : Migration
{
    public override void Up()
    {
        Create.Schema("vehicles_module");
    }

    public override void Down()
    {
        Delete.Schema("vehicles_module");
    }    
}