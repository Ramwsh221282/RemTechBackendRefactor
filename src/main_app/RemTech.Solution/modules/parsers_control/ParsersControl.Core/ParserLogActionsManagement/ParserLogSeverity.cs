using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLogActionsManagement;

public abstract record ParserLogSeverity(string Value)
{
    public sealed record Information() : ParserLogSeverity("Информация");
    public sealed record Error() : ParserLogSeverity("Ошибка");
    public sealed record InformationError() : ParserLogSeverity("Ошибка в данных");

    public static Result<ParserLogSeverity> Parse(string text)
    {
        return text.ToLower() switch
        {
            "info" => new Information(),
            "информация" => new Information(),
            "ошибка" => new Error(),
            "error" => new Error(),
            "info_error" => new InformationError(),
            "ошибка в данных" => new InformationError(),
            _ => RemTech.SharedKernel.Core.FunctionExtensionsModule.Error.Validation("Неподдерживаемый тип лога действия парсера")
        };
    }
}