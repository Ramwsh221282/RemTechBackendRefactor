using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class EditParserWhenWorkingError : IError
{
    private readonly Error _error = Error.Conflict(
        "Нельзя редактировать парсер в рабочем состоянии."
    );

    public Error Read() => _error;

    public static implicit operator Status(EditParserWhenWorkingError error) => error._error;

    public static implicit operator Status<IParserLink>(EditParserWhenWorkingError error) =>
        error._error;
}
