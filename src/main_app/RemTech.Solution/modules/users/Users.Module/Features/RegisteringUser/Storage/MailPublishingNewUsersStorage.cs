using Mailing.Module.Bus;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class MailPublishingNewUsersStorage(
    MailingBusPublisher publisher,
    INewUsersStorage storage
) : INewUsersStorage
{
    public async Task<bool> Save(
        string name,
        string email,
        string password,
        CancellationToken ct = default
    )
    {
        bool result = await storage.Save(name, email, password, ct);
        await publisher.Send(new MailingBusMessage(email, "Спасибо за регистрацию."), ct);
        return result;
    }
}
