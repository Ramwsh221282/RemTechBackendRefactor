﻿using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingStartedParser(IParsers source, IAsyncStartedParser inner)
    : IAsyncStartedParser
{
    public async Task<Status<IParser>> StartedAsync(
        AsyncStartParser start,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParser>(source)
            .WithLogicMethod(() => inner.StartedAsync(start, ct))
            .WithPutting(start)
            .WithReceivingMethod(p => p.Find(start.WhomStartId(), ct))
            .Process();
}
