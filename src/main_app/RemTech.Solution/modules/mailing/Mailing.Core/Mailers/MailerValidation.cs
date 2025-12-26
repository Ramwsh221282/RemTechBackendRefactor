namespace Mailing.Core.Mailers;

public static class MailerValidation
{
    extension(Mailer mailer)
    {
        public Mailer Validated()
        {
            mailer.Config.Validate();
            mailer.Domain.Validate();
            return mailer;
        }
    }
}