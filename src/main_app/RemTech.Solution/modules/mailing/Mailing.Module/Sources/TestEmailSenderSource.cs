using Mailing.Module.Contracts;
using Mailing.Module.Models;

namespace Mailing.Module.Sources;

public sealed class TestEmailSenderSource : IEmailSendersSource
{
    private readonly Dictionary<string, IEmailSender> _senders = new();

    public Task<IEmailSender> Get(string name, CancellationToken ct = default)
    {
        return Task.FromResult(_senders[name]);
    }

    public Task<bool> Save(IEmailSender sender, CancellationToken ct = default)
    {
        EmailSenderOutput print = sender.Print();
        _senders.Add(print.Name, sender);
        return Task.FromResult(true);
    }

    public Task<bool> Update(IEmailSender sender, CancellationToken ct = default)
    {
        EmailSenderOutput output = sender.Print();
        _senders[output.Name] = sender;
        return Task.FromResult(true);
    }

    public Task<bool> Remove(IEmailSender sender, CancellationToken ct = default)
    {
        EmailSenderOutput output = sender.Print();
        return Task.FromResult(_senders.Remove(output.Name));
    }

    public Task<IEnumerable<IEmailSender>> ReadAll(CancellationToken ct = default)
    {
        return Task.FromResult(_senders.Values.AsEnumerable());
    }

    public TestEmailSenderSource Add(string serviceName, string email, string password)
    {
        EmailSender sender = new(serviceName, email, password);
        _senders.Add(serviceName, sender);
        return this;
    }
}
