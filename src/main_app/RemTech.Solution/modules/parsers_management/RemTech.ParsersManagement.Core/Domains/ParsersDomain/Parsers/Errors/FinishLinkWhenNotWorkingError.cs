using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;

public sealed class FinishLinkWhenNotWorkingError : IError
{
    private readonly Error _error;

    public FinishLinkWhenNotWorkingError(IParser parser)
    {
        string message =
            $"Парсер ID: {parser.Identification().ReadId().GuidValue()}, название: {parser.Identification().ReadName().NameString().StringValue()} тип: {parser.Identification().ReadType().Read().StringValue()} домен: {parser.Identification().Domain().Read().NameString().StringValue()} не может закончить работу ссылку, поскольку парсер не в рабочем состоянии.";
        _error = Error.Conflict(message);
    }

    public Error Read() => _error;

    public static implicit operator Status<IParserLink>(FinishLinkWhenNotWorkingError error) =>
        error._error;
}
