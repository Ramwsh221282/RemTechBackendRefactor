namespace Mailers.Communication.State;

public interface IMailerMetadataStateSource
{
    void Accept(Guid id, string email, string smtpPassword);
}