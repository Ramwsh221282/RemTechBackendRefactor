namespace Mailing.Infrastructure.NpgSql;

public sealed class MailingModuleDbUpgrader : AbstractDatabaseUpgrader
{
    public MailingModuleDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(MailingModuleDbUpgrader).Assembly);
    }
}