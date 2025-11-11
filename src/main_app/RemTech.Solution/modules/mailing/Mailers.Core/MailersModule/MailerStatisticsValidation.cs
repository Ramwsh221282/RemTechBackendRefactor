namespace Mailers.Core.MailersModule;

public static class MailerStatisticsValidation
{
    extension(MailerStatistics statistics)
    {
        public Result<MailerStatistics> Validated()
        {
            if (statistics.IsLimitNegative()) return Validation("Лимит отправленных сообщений был отрицательным.");
            if (statistics.IsCurrentSentNegative()) return Validation("Число отправленных сообщений было отрицательное.");
            return statistics;
        }

        private bool IsLimitNegative()
        {
            return statistics.Limit < 0;
        }

        private bool IsCurrentSentNegative()
        {
            return statistics.SendCurrent < 0;
        }
    }
}