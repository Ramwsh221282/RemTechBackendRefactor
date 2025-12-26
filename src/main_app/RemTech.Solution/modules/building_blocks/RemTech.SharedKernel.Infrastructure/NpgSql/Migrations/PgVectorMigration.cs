using FluentMigrator;

namespace RemTech.SharedKernel.Infrastructure.NpgSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 1, hour: 1, minute: 1)]
public sealed class PgVectorMigration : Migration
{
    public override void Up() => Execute.Sql("CREATE EXTENSION IF NOT EXISTS vector;");
    public override void Down() => Execute.Sql("DROP EXTENSION IF EXISTS vector;");
}