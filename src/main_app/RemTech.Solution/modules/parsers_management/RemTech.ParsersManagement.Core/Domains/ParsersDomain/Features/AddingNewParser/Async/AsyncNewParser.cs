using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async;

public sealed class AsyncNewParser(INewParser inner) : IAsyncNewParser
{
    public Task<Status<IParser>> Register(AsyncAddNewParser add, CancellationToken ct = default) =>
        Task.FromResult(
            inner.Register(
                new AddNewParser(
                    add.WhatName(),
                    add.WhatType(),
                    new ParserServiceDomain(add.WhatDomain())
                )
            )
        );
}
