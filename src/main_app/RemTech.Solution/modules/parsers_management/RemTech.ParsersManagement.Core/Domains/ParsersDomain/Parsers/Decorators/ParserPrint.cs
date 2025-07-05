using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public sealed class ParserPrint(IParser parser) : IPrint
{
    private string _print = string.Empty;
    private readonly string LastRunDateString = new ShortDateString(
        parser.WorkSchedule().LastRun()
    ).Read();
    private readonly string NextRunDateString = new ShortDateString(
        parser.WorkSchedule().NextRun()
    ).Read();

    public string Read()
    {
        if (!string.IsNullOrWhiteSpace(_print))
            return _print;
        return _print = $"""
            Информация о парсере:
            ID: {parser.Identification().ReadId().GuidValue()}
            Название: {parser.Identification().ReadName().NameString().StringValue()}
            Тип: {parser.Identification().ReadType().Read().StringValue()}
            Домен: {parser.Domain().Read().NameString().StringValue()}
            Состояние: {parser.WorkState().Read().StringValue()}
            Последний раз работал: {LastRunDateString}
            Следующий раз работать: {NextRunDateString}
            Время ожидания между работой: {parser.WorkSchedule().WaitDays().Read()}
            """;
    }
}
