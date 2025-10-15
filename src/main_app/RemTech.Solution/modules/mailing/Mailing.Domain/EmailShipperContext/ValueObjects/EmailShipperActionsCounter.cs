namespace Mailing.Domain.EmailShipperContext.ValueObjects;

public readonly record struct EmailShipperActionsCounter
{
    public int Sended { get; }
    public int SendLimit { get; }

    public EmailShipperActionsCounter()
    {
        Sended = 0;
        SendLimit = 0;
    }

    private EmailShipperActionsCounter(int sended, int sendLimit)
    {
        Sended = sended;
        SendLimit = sendLimit;
    }

    public EmailShipperActionsCounter Increment()
    {
        return new EmailShipperActionsCounter(Sended + 1, SendLimit);
    }
}
