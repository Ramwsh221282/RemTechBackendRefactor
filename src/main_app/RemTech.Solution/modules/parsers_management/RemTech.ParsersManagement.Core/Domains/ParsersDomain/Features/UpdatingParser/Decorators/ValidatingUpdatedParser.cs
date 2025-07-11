﻿using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class ValidatingUpdatedParser(IUpdatedParser inner) : IUpdatedParser
{
    public Status<IParser> Updated(UpdateParser update) =>
        new ValidatingOperation(update).Process(() => inner.Updated(update));
}
