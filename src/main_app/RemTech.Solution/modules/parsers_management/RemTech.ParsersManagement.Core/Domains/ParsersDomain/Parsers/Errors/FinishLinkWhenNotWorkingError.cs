using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class FinishLinkWhenNotWorkingError : IError
{
    private readonly Error _error;

    public FinishLinkWhenNotWorkingError(IParser parser)
    {
        Guid id = parser.Identification().ReadId();
        string name = parser.Identification().ReadName();
        string type = parser.Identification().ReadType().Read();
        string domain = parser.Domain().Read();
        string message =
            $"Парсер ID: {id}, названием: {name} тип: {type} домен: {domain} не может закончить работу ссылку, поскольку парсер не в рабочем состоянии.";
        _error = (message, ErrorCodes.Conflict);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(FinishLinkWhenNotWorkingError error) =>
        error._error;
}
