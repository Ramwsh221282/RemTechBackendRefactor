using FluentMigrator;

namespace Identity.Infrastructure.Common;

[Migration(1767457600)]
public sealed class AccountsModuleOutboxMigration : Migration
{
    public override void Up() =>
        Create.Table("outbox").InSchema("identity_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("type").AsString(128).NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable()
            .WithColumn("created").AsDateTime().NotNullable()
            .WithColumn("sent").AsDateTime().Nullable()
            .WithColumn("payload").AsCustom("jsonb").NotNullable();

    public override void Down() =>
        Delete.Table("outbox").InSchema("identity_module");
}