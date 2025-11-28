using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.Infrastructure.Migrations;

public sealed class ParsersControlModuleDbUpgrader : AbstractDatabaseUpgrader
{
    public ParsersControlModuleDbUpgrader(IOptions<NpgSqlOptions> options) : base(options)
    {
        OfAssembly(typeof(ParsersControlModuleDbUpgrader).Assembly);
    }
}