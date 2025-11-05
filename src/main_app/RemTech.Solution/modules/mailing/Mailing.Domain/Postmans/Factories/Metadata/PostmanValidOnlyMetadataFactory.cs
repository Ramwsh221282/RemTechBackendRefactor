using Mailing.Domain.General;

namespace Mailing.Domain.Postmans.Factories.Metadata;

public sealed class PostmanValidOnlyMetadataFactory(IPostmanMetadataFactory origin)
    : PostmanMetadataFactoryEnvelope(origin)
{
    public override IPostmanMetadata Construct(Guid id, string email, string password)
    {
        ThrowIfNotValid(email, password);
        return base.Construct(id, email, password);
    }

    public override IPostmanMetadata Construct(string email, string password)
    {
        ThrowIfNotValid(email, password);
        return base.Construct(email, password);
    }

    private void ThrowIfNotValid(string email, string password)
    {
        if (!IsPasswordValid(password))
            throw new InvalidObjectStateException("SMTP ключ почтового отправителя пустой.");
        if (!IsEmailValid(email))
            throw new InvalidObjectStateException("Некорректный формат почты у почтового отправителя.");
    }

    private bool IsPasswordValid(string password) => !string.IsNullOrWhiteSpace(password);

    private bool IsEmailValid(string email) =>
        EmailContainsDomain(email) && EmailContainsSeparator(email) && EmailNotEmpty(email);

    private bool EmailContainsDomain(string email)
    {
        int atIndex = email.IndexOf('@');
        string domain = email[(atIndex + 1)..];
        return domain.Contains('.') && domain.IndexOf('.') != domain.Length - 1;
    }

    private bool EmailContainsSeparator(string email)
    {
        int atIndex = email.IndexOf('@');
        return atIndex > 0 && atIndex != email.Length - 1;
    }

    private bool EmailNotEmpty(string email) => !string.IsNullOrWhiteSpace(email);
}