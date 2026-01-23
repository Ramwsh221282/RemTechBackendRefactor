using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

[Migration(1766980872)]
public sealed class SparesSchemaMigration : Migration
{
	public override void Up() => Create.Schema("spares_module");

	public override void Down() => Delete.Schema("spares_module");
}
