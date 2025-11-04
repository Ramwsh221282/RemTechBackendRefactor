using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics.Factories;

public sealed class PostmanStatisticsValidatingFactory(IPostmanStatisticsFactory factory)
    : PostmanStatisticsFactoryEnvelope(factory)
{
    public Status<IPostmanSendingStatistics> Construct(Guid postmanId, int limit, int currentAmount)
    {
        if (postmanId == Guid.Empty)
            return Error.Validation(AppendPostfix("Идентификатор postman не указан."));
        if (limit <= 0)
            return Error.Validation(
                AppendPostfix("Ограничение максимальной отправки сообщений у postman не может быть 0"));
        if (currentAmount <= 0)
            AppendPostfix("Текущее число отправленных сообщений у postman не может быть 0");
        return base.Construct(postmanId, limit, currentAmount);
    }

    private static string AppendPostfix(string error) => $"Не удается создать статистику для postman." + " " + error;
}