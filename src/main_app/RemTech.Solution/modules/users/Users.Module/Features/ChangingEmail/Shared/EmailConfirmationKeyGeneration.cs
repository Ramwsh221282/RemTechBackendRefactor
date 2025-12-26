namespace Users.Module.Features.ChangingEmail.Shared;

internal sealed class EmailConfirmationKeyGeneration(ConfirmationEmailsCache cache)
{
    public async Task<Guid> Generate(Guid userId)
    {
        Guid confirmationKeyId = Guid.NewGuid();
        await cache.Create(confirmationKeyId, userId);
        return confirmationKeyId;
    }
}
