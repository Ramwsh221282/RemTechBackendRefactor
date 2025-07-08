using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Decorators;

public sealed class ValidatingNewParser(INewParser inner) : INewParser
{
    public Status<IParser> Register(AddNewParser add) =>
        new ValidatingOperation(add).Process(() => inner.Register(add));
}
