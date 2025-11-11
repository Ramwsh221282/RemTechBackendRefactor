namespace Mailers.Core.MailersModule;

public static class MailerStatisticsCreation
{
    extension(MailerStatistics)
    {
        public static Result<MailerStatistics> Construct(int limit, int current)
        {
            return new MailerStatistics(limit, current).Validated();
        }
        
        public static Result<MailerStatistics> Construct()
        {
            return new MailerStatistics(0, 0).Validated();
        }
    }
}