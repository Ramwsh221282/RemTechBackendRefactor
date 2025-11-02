using Mailing.Domain.EmailSendingContext.Ports;
using RemTech.Core.Shared.Monads;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.ValueObjects;

public readonly struct SenderServiceStatistics
{
    private readonly int _limit;
    private readonly int _currentAmount;

    public SenderServiceStatistics()
    {
        _limit = 0;
        _currentAmount = 0;
    }

    private SenderServiceStatistics(int limit, int sentMessagesAmount)
    {
        _limit = limit;
        _currentAmount = sentMessagesAmount;
    }

    public static Status<SenderServiceStatistics> Create(int sendMessageLimit, int sentMessagesAmount) =>
        from valid_limit in new ValidValue<int>(sendMessageLimit)
            .AddValidation(first => first > 0,
                Error.Validation("У сервиса отправки почты не может быть лимит сообщений меньше 0."))
            .Validate()
        from valid_amount in new ValidValue<int>(sentMessagesAmount)
            .AddValidation(second => second > 0,
                Error.Validation(
                    "У сервиса отправки почты не может быть количество отправленных сообщений меньше 0."))
            .Validate()
        select new SenderServiceStatistics(sendMessageLimit, sentMessagesAmount);

    public bool IsLimitReached() => _currentAmount > _limit;

    public Error StatisticsReachedError(SenderServiceInformation service) =>
        Error.Conflict("Превышен лимит на отправку сообщений {0} Лимит: {1} Текущий: {2}.",
            service.Fold<string>((name, _) => name),
            _limit,
            _currentAmount);

    public Func<SenderServiceStatistics> OnMessageSent()
    {
        int nextSendedAmount = _currentAmount + 1;
        int limit = _limit;
        return () => new SenderServiceStatistics(limit, nextSendedAmount);
    }

    public SenderServiceStatistics Reset() => new();

    public T Fold<T>(EmailSenderServiceStatisticsSink<T> use) => use(_limit, _currentAmount);
}