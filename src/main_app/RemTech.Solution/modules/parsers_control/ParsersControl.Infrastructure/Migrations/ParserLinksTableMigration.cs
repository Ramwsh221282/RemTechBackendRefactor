using FluentMigrator;

namespace ParsersControl.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы ссылок парсера.
/// </summary>
[Migration(202501050001)]
public sealed class ParserLinksTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию вверх.
	/// </summary>
	public override void Up()
	{
		Create
			.Table("parser_links")
			.InSchema("parsers_control_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey()
			.WithColumn("parser_id")
			.AsGuid()
			.NotNullable()
			.ForeignKey("fk_parser_links_registered_parsers", "parsers_control_module", "registered_parsers", "id")
			.WithColumn("name")
			.AsString(255)
			.NotNullable()
			.WithColumn("url")
			.AsString()
			.NotNullable()
			.WithColumn("elapsed_seconds")
			.AsInt64()
			.NotNullable()
			.WithColumn("processed")
			.AsInt32()
			.NotNullable()
			.WithColumn("is_active")
			.AsBoolean()
			.NotNullable();
		Execute.Sql(
			"CREATE INDEX IF NOT EXISTS idx_parser_links_parser_id ON parsers_control_module.parser_links(parser_id)"
		);
		Execute.Sql("CREATE INDEX IF NOT EXISTS idx_parser_links_name ON parsers_control_module.parser_links(name)");
		Execute.Sql("CREATE INDEX IF NOT EXISTS idx_parser_links_url ON parsers_control_module.parser_links(url)");
	}

	/// <summary>
	/// Выполняет миграцию вниз.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP INDEX IF EXISTS idx_parser_links_parser_id");
		Execute.Sql("DROP INDEX IF EXISTS idx_parser_links_name");
		Execute.Sql("DROP INDEX IF EXISTS idx_parser_links_url");
		Delete.Table("parser_links").InSchema("parsers_control_module");
	}
}
