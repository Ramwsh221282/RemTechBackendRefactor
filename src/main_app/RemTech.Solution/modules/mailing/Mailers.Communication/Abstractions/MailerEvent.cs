namespace Mailers.Communication.Abstractions;

public sealed class MailerEvent : IMailerStateSource
{
    private Optional<Guid> _id = None<Guid>();
    private Optional<string> _email = None<string>();
    private Optional<string> _password = None<string>();
    private Optional<int> _sendLimit = None<int>();
    private Optional<int> _sendCurrent = None<int>();
    
    public void Accept(Guid id, string email, string smtpPassword)
    {
        _id = Some(id);
        _email = Some(email);
        _password = Some(smtpPassword);
    }

    public void Accept(int sendLimit, int currentAmount)
    {
        _sendLimit = Some(sendLimit);
        _sendCurrent = Some(currentAmount);
    }

    public void TellTo(IMailerEventTarget target) =>
        target.Accept(_id, _email, _password, _sendLimit, _sendCurrent);
}