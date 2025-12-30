using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767097485)]
public sealed class CharacteristicsSeeding : Migration
{
    public override void Up()
    {
        Execute.EmbeddedScript("Vehicles.Infrastructure.Characteristics.SeedingImplementation.characteristics_seeding.sql");
    }

    public override void Down() { }
}