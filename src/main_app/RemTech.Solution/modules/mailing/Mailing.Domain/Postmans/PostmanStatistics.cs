using Mailing.Domain.General;
using Mailing.Domain.Postmans.Storing;

namespace Mailing.Domain.Postmans;

internal sealed class PostmanStatistics(int sendLimit, int currentSend) : IPostmanStatistics
{
    public PostmanStatistics() : this(0, 0)
    {
    }

    public void Save(IPostmanStatisticsStorage storage)
    {
        ThrowIfNotValid();
        storage.Save(sendLimit, currentSend);
    }

    public bool Sendable() =>
        sendLimit != currentSend;

    private void ThrowIfNotValid()
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