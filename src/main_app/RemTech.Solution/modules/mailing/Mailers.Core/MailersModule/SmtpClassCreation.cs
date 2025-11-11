namespace Mailers.Core.MailersModule;

public static class SmtpClassCreation
{
    extension (SmtpPassword)
    {
        public static Result<SmtpPassword> Construct(string password)
        {
            return new SmtpPassword(password).Validated();
        }
    }
}