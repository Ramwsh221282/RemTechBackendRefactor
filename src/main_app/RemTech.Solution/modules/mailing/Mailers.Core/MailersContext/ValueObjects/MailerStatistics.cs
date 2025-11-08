namespace Mailers.Core.MailersContext.ValueObjects;

public readonly struct MailerStatistics
{
    private readonly int _sendLimit;
    private readonly int _sendAtThisMoment;

    private MailerStatistics(int sendLimit, int sendAtThisMoment)
    {
        _sendLimit = sendLimit;
        _sendAtThisMoment = sendAtThisMoment;
    }

    public void SignRegistration(MailerEvent @event)
    {
        @event.Accept(_sendLimit, _sendAtThisMoment);
    }
    
    public MailerStatistics()
    {
        _sendLimit = 0;
        _sendAtThisMoment = 0;
    }

    public static Result<MailerStatistics> Create(int sendLimit, int sendAtThisMoment) =>
        Invariant
            .For(sendLimit, val => val >= 0)
            .BindError(Validation("Лимит отправленных сообещний отрицательный."))
            .SwitchTo(sendAtThisMoment, val => val >= 0)
            .BindError(Validation("Число отправленных сообещний отрицательное."))
            .Map(() => new MailerStatistics(sendLimit, sendAtThisMoment));

    public bool LimitReached()
    {
        return _sendAtThisMoment >= _sendLimit;
    }

    public MailerStatistics Increase()
    {
        var nextSendAtm = _sendAtThisMoment + 1;
        return new MailerStatistics(_sendLimit, nextSendAtm);
    }

    public MailerStatistics Reset()
    {
        return new MailerStatistics(_sendLimit, 0);
    }
}