using FluentMigrator;

namespace RemTech.SharedKernel.Infrastructure.Database;

// should be run first.
[Migration(100)]
public sealed class PgVectorMigration : Migration
{
	public const long Version = 20251201010100;

	public override void Up() => Execute.Sql("CREATE EXTENSION IF NOT EXISTS vector;");

	public override void Down() => Execute.Sql("DROP EXTENSION IF EXISTS vector;");
}
