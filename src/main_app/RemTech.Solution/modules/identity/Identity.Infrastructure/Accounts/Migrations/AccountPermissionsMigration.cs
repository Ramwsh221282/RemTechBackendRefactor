using FluentMigrator;

namespace Identity.Infrastructure.Accounts.Migrations;

[Migration(1767457500)]
public sealed class AccountPermissionsMigration : Migration
{
	public override void Up() =>
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS identity_module.account_permissions (
			    account_id UUID NOT NULL,
			    permission_id UUID NOT NULL,
			    PRIMARY KEY (account_id, permission_id),
			    FOREIGN KEY (account_id) REFERENCES identity_module.accounts (id),
			    FOREIGN KEY (permission_id) REFERENCES identity_module.permissions (id)
			);
			"""
		);

	public override void Down() => Execute.Sql("DROP TABLE IF EXISTS identity_module.account_permissions;");
}
