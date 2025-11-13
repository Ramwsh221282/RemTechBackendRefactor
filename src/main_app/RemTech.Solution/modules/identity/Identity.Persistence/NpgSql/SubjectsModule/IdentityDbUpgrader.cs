using Microsoft.Extensions.Options;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule;

public sealed class IdentityDbUpgrader : AbstractDatabaseUpgrader
{
    public IdentityDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(IdentityDbUpgrader).Assembly);
    }
}