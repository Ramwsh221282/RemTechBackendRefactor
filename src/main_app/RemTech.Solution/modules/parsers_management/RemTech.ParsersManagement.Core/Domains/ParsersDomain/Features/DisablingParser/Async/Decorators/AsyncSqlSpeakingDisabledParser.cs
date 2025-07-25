﻿using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingDisabledParser(IParsers parsers, IAsyncDisabledParser inner)
    : IAsyncDisabledParser
{
    public async Task<Status<IParser>> Disable(
        AsyncDisableParser disable,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParser>(parsers)
            .WithReceivingMethod(s => s.Find(disable.WhomDisableId(), ct))
            .WithLogicMethod(() => inner.Disable(disable, ct))
            .WithPutting(disable)
            .Process();
}
