namespace Mailers.Core.MailersModule;

public static class MailerIdValidation
{
    extension(MailerId id)
    {
        public Result<MailerId> Validated()
        {
            if (id.IsEmpty()) return Validation("Идентификатор пустой.");
            return id;
        }
        
        
        public bool IsEmpty()
        {
            return id.Value == Guid.Empty;
        }
    }
}