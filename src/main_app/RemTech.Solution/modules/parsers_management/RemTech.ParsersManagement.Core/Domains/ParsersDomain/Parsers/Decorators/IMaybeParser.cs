namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public interface IMaybeParser
{
    void Put(IParser parser);
    IParser Take();
}
