using Mailing.Domain.General;

namespace Mailing.Domain.Postmans.Factories.Statistics;

public sealed class PostmanValidOnlyStatisticsFactory(IPostmanStatisticsFactory factory)
    : PostmanStatisticsFactoryEnvelope(factory)
{
    public override IPostmanStatistics Construct(int sendLimit, int currentSend)
    {
        ThrowIfNotValid(sendLimit, currentSend);
        return base.Construct(sendLimit, currentSend);
    }

    private void ThrowIfNotValid(int sendLimit, int currentSend)
    {
        if (!ValueNotNegative(sendLimit))
            throw new InvalidObjectStateException(
                "Лимит статистики отправки у почтового отправителя не должен быть отрицательным.");
        if (!ValueNotNegative(currentSend))
            throw new InvalidObjectStateException(
                "Лимит текущее число отправки у почтового отправителя не должен быть отрицательным.");
    }

    private bool ValueNotNegative(int value) => value >= 0;
}