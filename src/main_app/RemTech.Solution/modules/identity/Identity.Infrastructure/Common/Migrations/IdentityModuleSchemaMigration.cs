using FluentMigrator;

namespace Identity.Infrastructure.Common.Migrations;

[Migration(1767457100)]
public sealed class IdentityModuleSchemaMigration : Migration
{
	public override void Up()
	{
		Create.Schema("identity_module");
	}

	public override void Down()
	{
		Delete.Schema("identity_module");
	}
}
