using FluentMigrator;

namespace Identity.Infrastructure.Tokens.Migrations;

[Migration(1767696579)]
public sealed class AccessTokensTableMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE TABLE IF NOT EXISTS identity_module.access_tokens 
                    (
                        token_id UUID NOT NULL primary key,
                        raw_token TEXT NOT NULL,
                        expires_at BIGINT NOT NULL,
                        created_at BIGINT NOT NULL,
                        email VARCHAR(256) NOT NULL,
                        login VARCHAR(256) NOT NULL,
                        user_id UUID NOT NULL,
                        raw_permissions TEXT NOT NULL,
                        is_expired BOOLEAN NOT NULL
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS identity_module.access_tokens;");
    }
}