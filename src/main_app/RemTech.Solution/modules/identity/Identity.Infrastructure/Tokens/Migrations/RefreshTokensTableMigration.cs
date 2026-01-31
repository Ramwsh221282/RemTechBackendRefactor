using FluentMigrator;

namespace Identity.Infrastructure.Tokens.Migrations;

/// <summary>
/// Миграция для создания таблицы refresh_tokens модуля identity.
/// </summary>
[Migration(1767695842)]
public sealed class RefreshTokensTableMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая таблицу refresh_tokens модуля identity.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS identity_module.refresh_tokens 
			(
			    account_id UUID NOT NULL primary key,
			    token_value TEXT NOT NULL,
			    expires_at BIGINT NOT NULL,
			    created_at BIGINT NOT NULL
			);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу refresh_tokens модуля identity.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS identity_module.refresh_tokens;");
	}
}
