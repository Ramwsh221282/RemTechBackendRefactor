using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink;

public sealed class NewParserLink : INewParserLink
{
    public Status<IParserLink> Register(AddParserLink addLink)
    {
        IParser parser = addLink.TakeOwner();
        ParserLinkIdentity identity = new(parser, new Name(addLink.WhatName()));
        IParserLink link = new ParserLink(identity, new ParserLinkUrl(addLink.WhatUrl()));
        return parser.Put(link);
    }
}
