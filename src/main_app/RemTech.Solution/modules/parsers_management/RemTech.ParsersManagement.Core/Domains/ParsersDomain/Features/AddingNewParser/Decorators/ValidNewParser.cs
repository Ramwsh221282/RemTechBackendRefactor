using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Decorators;

public sealed class ValidNewParser(INewParser inner) : INewParser
{
    public Status<IParser> Register(AddNewParser add)
    {
        Status<IParser> created = inner.Register(add);
        return created.IsSuccess ? new ValidParser(created.Value) : created.Error;
    }
}
