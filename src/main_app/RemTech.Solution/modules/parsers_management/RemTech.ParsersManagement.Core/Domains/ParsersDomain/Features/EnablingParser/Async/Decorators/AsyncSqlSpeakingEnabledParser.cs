﻿using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;

public sealed class AsyncSqlSpeakingEnabledParser(IParsers source, IAsyncEnabledParser inner)
    : IAsyncEnabledParser
{
    public async Task<Status<IParser>> EnableAsync(
        AsyncEnableParser enable,
        CancellationToken ct = default
    ) =>
        await new TransactionOperatingParser<IParser>(source)
            .WithReceivingMethod(s => s.Find(enable.WhomEnable(), ct))
            .WithLogicMethod(() => inner.EnableAsync(enable, ct))
            .WithPutting(enable)
            .Process();
}
