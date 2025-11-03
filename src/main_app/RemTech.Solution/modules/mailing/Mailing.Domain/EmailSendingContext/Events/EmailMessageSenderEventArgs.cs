namespace Mailing.Domain.EmailSendingContext.Events;

public sealed class EmailMessageSenderEventArgs
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly string _password;
    private readonly int _port;
    private readonly int _limit;
    private readonly int _currentSent;

    internal EmailMessageSenderEventArgs(EmailSender sender) :
        this(FromSender(sender))
    {
    }

    public T Fold<T>(Func<Guid, string, string, int, int, int, T> foldFn) =>
        foldFn(_id, _email, _password, _port, _limit, _currentSent);

    public void Fold(Action<Guid, string, string, int, int, int> foldFn) =>
        foldFn(_id, _email, _password, _port, _limit, _currentSent);

    internal EmailMessageSenderEventArgs(
        Guid id,
        string email,
        string password,
        int port,
        int limit,
        int currentSent
    )
    {
        _id = id;
        _email = email;
        _password = password;
        _port = port;
        _limit = limit;
        _currentSent = currentSent;
    }

    internal EmailMessageSenderEventArgs(EmailMessageSenderEventArgs ea)
        : this(ea._id, ea._email, ea._password, ea._port, ea._limit,
            ea._currentSent)
    {
    }

    internal static EmailMessageSenderEventArgs FromSender(EmailSender sender) =>
        FromTuple(sender.ProjectTo((id, email, _, password, limit, sent, port) =>
            (id, email, password, port, limit, sent)));

    internal static EmailMessageSenderEventArgs FromTuple(
        (
            Guid id,
            string email,
            string password,
            int limit,
            int sent,
            int port
            )
            data
    ) => new(data.id, data.email, data.password, data.limit, data.sent, data.port);
}