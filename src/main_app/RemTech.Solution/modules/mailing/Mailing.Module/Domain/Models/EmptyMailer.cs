namespace Mailing.Module.Domain.Models;

internal sealed class EmptyMailer : IMailer
{
    public Task Save(IMailersStorage mailersStorage, CancellationToken ct = default) =>
        throw new NotFoundException("Postman не найден.");
}