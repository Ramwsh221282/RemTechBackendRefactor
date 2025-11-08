namespace Mailers.Communication.Abstractions;

public abstract class CallbackableMailerEventTarget : IMailerEventTarget
{
    private readonly TaskCompletionSource<Result<Unit>> _tcs = new();
    private Optional<IMailerEventSource> _eventSource = None<IMailerEventSource>();
    protected Optional<Guid> _id = None<Guid>();
    protected Optional<string> _email = None<string>();
    protected Optional<string> _password = None<string>();
    protected Optional<int> _sendLimit = None<int>();
    protected Optional<int> _sendAtThisMoment = None<int>();

    public void Listen(IMailerEventSource source)
    {
        if (_eventSource.HasValue) return;
        source.Accept(this);
        _eventSource = Some(source);
    }

    public void Accept(
        Optional<Guid> id, 
        Optional<string> email, 
        Optional<string> password, 
        Optional<int> sendLimit, 
        Optional<int> sendCurrent)
    {
        _id = id;
        _email = email;
        _password = password;
        _sendLimit = sendLimit;
        _sendAtThisMoment = sendCurrent;
    }

    public virtual void Notify(MailerEvent @event)
    {
        if (!_eventSource.HasValue)
            throw new InvalidOperationException($"Источник событий не был прикреплен {nameof(CallbackableMailerEventTarget)}.");
        @event.TellTo(this);
    }

    public async Task<Result<Unit>> Read()
    {
        return await _tcs.Task;
    }
    
    protected void Complete(Result<Unit> result)
    {
        bool set = _tcs.TrySetResult(result);
        int a = 0;
    }
}