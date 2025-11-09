using Mailing.Module.Traits;
using RemTech.Core.Shared.Exceptions;

namespace Mailing.Module.MailersContext.MetadataContext.Factories;

internal sealed class MailersMetadataFactory : IFactoryOf<MailersMetadataFactory.Input, MailerMetadata>
{
    public MailerMetadata Create(Input input)
    {
        input.Validate();
        return input.Create();
    }

    public sealed class Input(Guid id, string email, string password)
        : FactoryInput, IValidatable, ICreatorOf<MailerMetadata>
    {
        public MailerMetadata Create()
        {
            return new MailerMetadata(id, email, password);
        }

        public void Validate()
        {
            if (id == Guid.Empty)
                throw new InvalidObjectStateException("Идентификатор был пустым.");
            if (string.IsNullOrEmpty(email))
                throw new InvalidObjectStateException("Почта была пустой.");
            if (string.IsNullOrEmpty(password))
                throw new InvalidObjectStateException("SMTP пароль не указан.");
        }
    }
}