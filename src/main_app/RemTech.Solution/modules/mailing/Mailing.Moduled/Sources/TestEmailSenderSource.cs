using Mailing.Moduled.Contracts;
using Mailing.Moduled.Models;

namespace Mailing.Moduled.Sources;

public sealed class TestEmailSenderSource : IEmailSendersSource
{
    private readonly Dictionary<string, IEmailSender> _senders = new();

    public Task<IEmailSender> Get(string email, CancellationToken ct = default)
    {
        return Task.FromResult(_senders[email]);
    }

    public Task<bool> Save(IEmailSender sender, CancellationToken ct = default)
    {
        EmailSenderOutput print = sender.Print();
        _senders.Add(print.Name, sender);
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

    public TestEmailSenderSource Add(string email, string password)
    {
        EmailSender sender = new(email, password);
        _senders.Add(email, sender);
        return this;
    }
}