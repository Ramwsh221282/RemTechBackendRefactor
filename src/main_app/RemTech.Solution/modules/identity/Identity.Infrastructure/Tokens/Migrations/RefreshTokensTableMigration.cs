using FluentMigrator;

namespace Identity.Infrastructure.Tokens.Migrations;

[Migration(1767695842)]
public sealed class RefreshTokensTableMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE TABLE IF NOT EXISTS identity_module.refresh_tokens 
                    (
                        account_id UUID NOT NULL primary key,
                        token_value TEXT NOT NULL,
                        expires_at BIGINT NOT NULL,
                        created_at BIGINT NOT NULL
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS identity_module.refresh_tokens;");
    }
}