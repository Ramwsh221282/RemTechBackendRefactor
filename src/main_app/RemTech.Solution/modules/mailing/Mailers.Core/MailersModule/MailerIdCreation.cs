namespace Mailers.Core.MailersModule;

public static class MailerIdCreation
{
    extension(MailerId)
    {
        public static Result<MailerId> Construct(Guid id)
        {
            return new MailerId(id).Validated();
        }
        
        public static Result<MailerId> Construct(Optional<Guid> id)
        {
            return id.HasValue ? Construct(id.Value) : new MailerId(Guid.NewGuid());
        }
    }
}