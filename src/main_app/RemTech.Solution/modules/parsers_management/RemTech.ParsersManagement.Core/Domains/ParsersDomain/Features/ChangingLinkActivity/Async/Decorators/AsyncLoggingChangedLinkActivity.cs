using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;

public sealed class AsyncLoggingChangedLinkActivity(
    ICustomLogger logger,
    IAsyncChangedLinkActivity inner
) : IAsyncChangedLinkActivity
{
    public async Task<Status<IParserLink>> AsyncChangedActivity(
        AsyncChangeLinkActivity change,
        CancellationToken ct = default
    ) =>
        await new AsyncLoggingOperation<Status<IParserLink>>(
            logger,
            string.Intern("Асинхронное изменение активности ссылки.")
        ).Log(inner.AsyncChangedActivity(change, ct));
}
