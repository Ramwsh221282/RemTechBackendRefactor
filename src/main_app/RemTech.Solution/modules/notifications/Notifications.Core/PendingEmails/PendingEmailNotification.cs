using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.PendingEmails;

public sealed class PendingEmailNotification(Guid id, string recipient, string subject, string body, bool wasSent)
{
    private PendingEmailNotification(PendingEmailNotification origin)
        : this(origin.Id, origin.Recipient, origin.Subject, origin.Body, origin.WasSent) { }

    public Guid Id { get; } = id;
    public string Recipient { get; } = recipient;
    public string Subject { get; } = subject;
    public string Body { get; } = body;
    public bool WasSent { get; private set; } = wasSent;

    public void MarkSent() => WasSent = true;

    public static Result<PendingEmailNotification> CreateNew(string recipient, string subject, string body)
    {
        List<string> errors = [];
        if (string.IsNullOrWhiteSpace(recipient))
            errors.Add("Почта поле не может быть пустой");
        if (string.IsNullOrWhiteSpace(subject))
            errors.Add("Тема письма не может быть пустой");
        if (string.IsNullOrWhiteSpace(body))
            errors.Add("Текст письма не может быть пустым");
        if (errors.Count > 0)
        {
            string message = string.Join(", ", errors);
            return Error.Validation(message);
        }

        return Result.Success(new PendingEmailNotification(Guid.NewGuid(), recipient, subject, body, false));
    }

    public PendingEmailNotification Copy() => new(this);
}
