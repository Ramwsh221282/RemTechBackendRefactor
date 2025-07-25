using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;

public sealed class AsyncSqlSpeakingFinishedParserLink(
    IParsers parsers,
    IAsyncFinishedParserLink inner
) : IAsyncFinishedParserLink
{
    public async Task<Status<IParserLink>> AsyncFinished(
        AsyncFinishParserLink finish,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParserLink>(parsers)
            .WithReceivingMethod(s => s.Find(finish.WhatOwnerId(), ct))
            .WithLogicMethod(() => inner.AsyncFinished(finish, ct))
            .WithPutting(finish)
            .Process();
}
