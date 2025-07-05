using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser;

public sealed class NewParser : INewParser
{
    public Status<IParser> Register(AddNewParser add) =>
        new Parser(add.WhatName(), add.WhatType(), add.WhatDomain());
}
