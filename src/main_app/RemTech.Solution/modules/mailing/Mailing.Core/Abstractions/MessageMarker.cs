using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Core.Abstractions;

public interface IMessage;

public interface IAggregatedMessageListener;

public interface IAsyncMessageListener<in TMessage>
{
    Task React(TMessage message, CancellationToken ct = default);
}

public interface IMessageListener<in TMessage> where TMessage : IMessage
{
    void React(TMessage message);
}

public sealed record ErroredMessage(string MessageType, string Error) : IMessage;
public sealed record HandledMessage(string MessageType) : IMessage;

public sealed class MessagesLogger
    : IAggregatedMessageListener,
    IMessageListener<ErroredMessage>,
    IMessageListener<HandledMessage>
{
    private readonly IMessageListener<ErroredMessage> _errors;
    private readonly IMessageListener<HandledMessage> _handled;
    
    public void React(ErroredMessage message)
    {
        _errors.React(message);
    }

    public void React(HandledMessage message)
    {
        _handled.React(message);
    }

    public MessagesLogger(
        IMessageListener<ErroredMessage> errors,
        IMessageListener<HandledMessage> handled,
        SyncMessagesChoreography? choreography = null)
    {
        _errors = errors;
        _handled = handled;
        choreography?.RegisterListener<ErroredMessage>(this);
        choreography?.RegisterListener<HandledMessage>(this);
    }
}

public class ErroredMessagesLogger : IMessageListener<ErroredMessage>
{
    public void React(ErroredMessage message)
    {
        Console.WriteLine($"{message.MessageType}: ERROR - {message.Error}");
    }
}

public class HandledMessagesLogger : IMessageListener<HandledMessage>
{
    public void React(HandledMessage message)
    {
        Console.WriteLine($"{message.MessageType}: Reacted.");
    }
}

public sealed record ChoreographyProcess(Type MessageType, Action Action);
public sealed record AsyncChoreographyProcess(Type MessageType, Func<CancellationToken, Task> Action);

public sealed class AsyncMessagesChoreography(AsyncMessagesChoreography? other = null)
{
    private readonly Dictionary<Type, List<object>> _subscriptions = [];
    private readonly Queue<AsyncChoreographyProcess> _processes = [];

    public void SendMessage<TMessage>(in TMessage message) where TMessage : IMessage
    {
        IAsyncMessageListener<TMessage>[] listeners = GetMessageListeners<TMessage>();
        if (listeners.Length == 0) return;
        EnqueueForProcess(message, listeners);
    }

    public void RegisterListener<TMessage>(in IAsyncMessageListener<TMessage> listener) where TMessage : IMessage
    {
        Type messageType = typeof(TMessage);
        RegisterListener(messageType, listener);
    }
    
    public void RegisterListener(in IAggregatedMessageListener listener)
    {
        Type[] interfaces = listener
            .GetType()
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncMessageListener<>)).ToArray();

        foreach (var @interface in interfaces)
        {
            Type messageType = @interface.GetGenericArguments()[0];
            RegisterListener(messageType, listener);
        }
    }
    
    public async Task ProcessMessages(CancellationToken ct = default)
    {
        while (_processes.Count > 0)
        {
            AsyncChoreographyProcess process = _processes.Dequeue();
            await Invoke(process, ct);
        }
    }
    
    private async Task Invoke(AsyncChoreographyProcess process, CancellationToken ct)
    {
        string messageName = process.MessageType.Name;
        Func<CancellationToken, Task> action = process.Action;
        try
        {
            await action(ct);
            other?.SendMessage(new HandledMessage(messageName));
        }
        catch(MessageException ex)
        {
            other?.SendMessage(new ErroredMessage(messageName, ex.Error));
        }
    }

    private void RegisterListener(in Type messageType, in object listener)
    {
        if (!_subscriptions.TryGetValue(messageType, out List<object>? subscriptions))
        {
            subscriptions = [];
            _subscriptions[messageType] = subscriptions;
        }
        
        subscriptions.Add(listener);
    }
    
    private IAsyncMessageListener<TMessage>[] GetMessageListeners<TMessage>() where TMessage : IMessage
    {
        Type messageType = typeof(TMessage);
        return !_subscriptions.TryGetValue(messageType, out List<object>? subscriptions) 
            ? [] 
            : subscriptions.Cast<IAsyncMessageListener<TMessage>>().ToArray();
    }
    
    private void EnqueueForProcess<TMessage>(
        TMessage message, 
        IEnumerable<IAsyncMessageListener<TMessage>> listeners)
        where TMessage : IMessage
    {
        Type messageType = typeof(TMessage);
        foreach (IAsyncMessageListener<TMessage> listener in listeners)
            _processes.Enqueue(new AsyncChoreographyProcess(messageType, (ct) => listener.React(message, ct)));
    }
}

public class SyncMessagesChoreography(SyncMessagesChoreography? other = null)
{
    private readonly Dictionary<Type, List<object>> _subscriptions = [];
    private readonly Queue<ChoreographyProcess> _processes = [];

    public void SendMessage<TMessage>(in TMessage message) where TMessage : IMessage
    {
        IMessageListener<TMessage>[] listeners = GetMessageListeners<TMessage>();
        if (listeners.Length == 0) return;
        EnqueueForProcess(message, listeners);
    }

    public void RegisterListener<TMessage>(in IMessageListener<TMessage> listener) where TMessage : IMessage
    {
        Type messageType = typeof(TMessage);
        RegisterListener(messageType, listener);
    }
    
    public void RegisterListener(in IAggregatedMessageListener listener)
    {
        Type[] interfaces = listener
            .GetType()
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageListener<>)).ToArray();

        foreach (var @interface in interfaces)
        {
            Type messageType = @interface.GetGenericArguments()[0];
            RegisterListener(messageType, listener);
        }
    }
    
    public void ProcessMessages()
    {
        while (_processes.Count > 0)
        {
            ChoreographyProcess process = _processes.Dequeue();
            Invoke(process);
        }
    }
    
    private void Invoke(ChoreographyProcess process)
    {
        string messageName = process.MessageType.Name;
        Action action = process.Action;
        try
        {
            action();
            other?.SendMessage(new HandledMessage(messageName));
        }
        catch(MessageException ex)
        {
            other?.SendMessage(new ErroredMessage(messageName, ex.Error));
        }
    }

    private void RegisterListener(in Type messageType, in object listener)
    {
        if (!_subscriptions.TryGetValue(messageType, out List<object>? subscriptions))
        {
            subscriptions = [];
            _subscriptions[messageType] = subscriptions;
        }
        
        subscriptions.Add(listener);
    }
    
    private IMessageListener<TMessage>[] GetMessageListeners<TMessage>() where TMessage : IMessage
    {
        Type messageType = typeof(TMessage);
        return !_subscriptions.TryGetValue(messageType, out List<object>? subscriptions) 
            ? [] 
            : subscriptions.Cast<IMessageListener<TMessage>>().ToArray();
    }
    
    private void EnqueueForProcess<TMessage>(
        TMessage message, 
        IEnumerable<IMessageListener<TMessage>> listeners)
        where TMessage : IMessage
    {
        Type messageType = typeof(TMessage);
        foreach (IMessageListener<TMessage> listener in listeners)
            _processes.Enqueue(new ChoreographyProcess(messageType, () => listener.React(message)));
    }
}