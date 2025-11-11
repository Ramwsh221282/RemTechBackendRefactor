namespace Mailers.Core.MailersContext;

public readonly record struct MailerStatistics(int Limit, int SendCurrent)
{
    public bool LimitReached()
    {
        bool limitReached = SendCurrent >= Limit;
        return limitReached;
    }

    public MailerStatistics Increased()
    {
        MailerStatistics updated = this with { SendCurrent = SendCurrent + 1 };
        return updated;
    }

    public MailerStatistics Reset()
    {
        MailerStatistics updated = this with { SendCurrent = 0 };
        return updated;
    }
}