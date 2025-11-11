namespace Mailers.Core.MailersModule;

public readonly record struct MailerStatistics(int Limit, int SendCurrent)
{
    public bool LimitReached()
    {
        bool limitReached = SendCurrent >= Limit;
        return limitReached;
    }

    public MailerStatistics Increased()
    {
        int nextIncrease = SendCurrent + 1;
        MailerStatistics updated = this with { SendCurrent = nextIncrease };
        return updated;
    }

    public MailerStatistics Reset()
    {
        MailerStatistics updated = this with { SendCurrent = 0 };
        return updated;
    }
}