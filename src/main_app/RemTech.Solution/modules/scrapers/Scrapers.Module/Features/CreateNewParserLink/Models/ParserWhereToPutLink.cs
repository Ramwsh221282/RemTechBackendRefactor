using System.Text.RegularExpressions;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;

namespace Scrapers.Module.Features.CreateNewParserLink.Models;

internal sealed record ParserWhereToPutLink(string Name, string Type, string State, string Domain)
{
    public ParserWithNewLink Put(string name, string url)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ParserWhereToPutLinkNameEmptyException();
        if (string.IsNullOrWhiteSpace(name))
            throw new ParserWhereToPutLinkNameEmptyException();
        if (string.IsNullOrWhiteSpace(State))
            throw new UnkownParserStateException();
        if (!url.Contains(Domain))
            throw new ParserLinkDomainDoesntMatchException(url, Domain);
        if (State == "Работает")
            throw new CannotPutLinkToWorkingParserException();
        if (Type == "Техника" || Type == "Запчасти")
            return new ParserWithNewLink(this, NewParserLink.Create(name, url, this));
        throw new ParserWhereToPutLinkUnsupportedTypeException(Type);
    }
}
