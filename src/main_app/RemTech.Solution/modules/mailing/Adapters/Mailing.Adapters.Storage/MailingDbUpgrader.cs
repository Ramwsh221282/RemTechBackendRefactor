using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Adapters.Storage;

public sealed class MailingDbUpgrader : AbstractDatabaseUpgrader
{
    public MailingDbUpgrader(IOptions<DatabaseOptions> options) : base(options) =>
        OfAssembly(typeof(MailingDbUpgrader).Assembly);
}