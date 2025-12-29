namespace Identity.Infrastructure;

public sealed class IdentityModuleDbUpgrader : AbstractDatabaseUpgrader
{
    public IdentityModuleDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(IdentityModuleDbUpgrader).Assembly);
    }
}