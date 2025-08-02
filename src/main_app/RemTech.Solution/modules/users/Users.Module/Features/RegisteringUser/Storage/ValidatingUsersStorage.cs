using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class ValidatingUsersStorage(IValidation validation, IUsersStorage origin)
    : IUsersStorage
{
    public Task<bool> Save(
        string name,
        string email,
        string password,
        CancellationToken ct = default
    )
    {
        validation.Check();
        return origin.Save(name, email, password, ct);
    }
}
