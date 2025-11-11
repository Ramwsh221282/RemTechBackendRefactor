using Mailers.Core.MailersContext;
using RemTech.Primitives.Extensions;

namespace Mailers.Application.Features;

public sealed record CreateMailerStatisticsFunctionArgument(int? Limit = null, int? SendCurrent = null)
    : IFunctionArgument;

public sealed class
    CreateMailerStatisticsFunction : IFunction<CreateMailerStatisticsFunctionArgument, Result<MailerStatistics>>
{
    public Result<MailerStatistics> Invoke(CreateMailerStatisticsFunctionArgument argument)
    {
        int? limit = argument.Limit;
        int? sendCurrent = argument.SendCurrent;
        return (limit, sendCurrent) switch
        {
            (null, null) => new MailerStatistics(),
            (_, null) => Validation("Число отправленных сообщений не указано."),
            (null, _) => Validation("Лимит отправленных сообщений не указано."),
            (not null, not null) when Numbers.IsNegative(limit.Value) => Validation(
                "Лимит отправленных сообщений отрицательный"),
            (not null, not null) when Numbers.IsNegative(sendCurrent.Value) => Validation(
                "Число отправленных сообщений отрицательное"),
            _ => new MailerStatistics(limit.Value, sendCurrent.Value)
        };
    }
}