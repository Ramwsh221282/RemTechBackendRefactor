namespace Mailing.Core.Mailers;

public static class MailerDomainManagement
{
    extension(MailerDomain domain)
    {
        public MailerDomain WithResolvedService()
        {
            return domain switch
            {
                _ when domain.ContainsPrefix("@gmail.com") => 
                    domain with { Service = "Google", SendLimit = 500, SmtpHost = "smtp.gmail.com" },
                
                _ when domain.ContainsPrefix("@mail.ru") => 
                    domain with { Service = "MailRu", SendLimit = 500, SmtpHost = "smtp.mail.ru"},
                
                _ when domain.ContainsPrefix("@yandex.ru") => 
                    domain with { Service = "Yandex", SendLimit = 300, SmtpHost =  "smtp.yandex.ru" },
                
                _ => domain with { Service = "", SmtpHost = ""}
            };
        }
        
        private bool ContainsPrefix(string prefix)
        {
            string email = domain.Email.Value;
            return !string.IsNullOrWhiteSpace(email) && email.EndsWith(prefix);
        }
    }
}