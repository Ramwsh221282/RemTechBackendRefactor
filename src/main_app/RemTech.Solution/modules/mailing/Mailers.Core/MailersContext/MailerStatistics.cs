namespace Mailers.Core.MailersContext;

public readonly record struct MailerStatistics(int Limit, int SendCurrent)
{
    public bool LimitReached()
    {
        return SendCurrent > Limit;
    }

    public MailerStatistics Increased()
    {
        return this with { SendCurrent = SendCurrent + 1 };
    }

    public MailerStatistics Reset()
    {
        return this with { SendCurrent = 0 };
    }
}