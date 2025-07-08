using RemTech.Core.Shared.Primitives;

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
            ID: {(Guid)parser.Identification().ReadId()}
            Название: {(string)parser.Identification().ReadName().NameString()}
            Тип: {(string)parser.Identification().ReadType().Read()}
            Домен: {(string)parser.Domain().Read().NameString()}
            Состояние: {(string)parser.WorkState()}
            Последний раз работал: {LastRunDateString}
            Следующий раз работать: {NextRunDateString}
            Время ожидания между работой: {(int)parser.WorkSchedule().WaitDays()}
            """;
    }
}
