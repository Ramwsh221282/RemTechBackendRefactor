using Mailers.Core.EmailsModule;

namespace Mailers.Core.MailersModule;

public static class MailerMetadataCreation
{
    extension(MailerMetadata)
    {
        public static Result<MailerMetadata> Create(string email, Optional<Guid> id)
        {
            Result<Email> emailRes = Email.Construct(email);
            Result<MailerId> idRes = MailerIdCreation.Construct(id);
            return emailRes.Continue(idRes).Map(() => new MailerMetadata(idRes, emailRes));
        }
    }
}