using Mailing.Domain.General;
using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Domain.Implementations;

public sealed class WritePostmanStatisticsDomain() : IWritePostmanStatisticsDomainCommand
{
    private const string NegativeLimit = "Лимит отправленных сообщений не может быть отрицательным.";
    private const string NegativeSent = "Количество отправленных сообщений не может быть отрицательным.";
    public void Execute(in int sendLimit, in int currentSend) => Invalidate(sendLimit, currentSend);

    private static void Invalidate(int sendLimit, int currentSend)
    {
        if (sendLimit < 0) throw new InvalidObjectStateException(NegativeLimit);
        if (currentSend < 0) throw new InvalidObjectStateException(NegativeSent);
    }
}