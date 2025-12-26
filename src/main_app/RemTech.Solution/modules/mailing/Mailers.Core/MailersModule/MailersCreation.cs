namespace Mailers.Core.MailersModule;

public static class MailersCreation
{
    extension(Mailer)
    {
        public static Result<Mailer> Construct(string email, string smtpPassword, Optional<Guid> id)
        {
            Result<MailerMetadata> metadata = MailerMetadata.Create(email, id);
            Result<SmtpPassword> passwordRes = SmtpPassword.Construct(smtpPassword);
            Result<MailerStatistics> statistics = MailerStatistics.Construct();
            return metadata
                .Continue(passwordRes)
                .Continue(statistics)
                .Map(() => new Mailer(metadata, passwordRes, statistics));
        }
        
        public static Result<Mailer> Construct(
            string email, 
            string smtpPassword, 
            int sendLimit, 
            int sendCurrent, 
            Optional<Guid> id)
        {
            Result<MailerStatistics> statistics = MailerStatistics.Construct(sendLimit, sendCurrent);
            Result<Mailer> mailer = Construct(email, smtpPassword, id);
            return statistics.Continue(mailer).Map(() => mailer.Value with { Statistics = statistics });
        }
    }
}