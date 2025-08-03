using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Storage;

internal sealed class ValidatingNewUsersStorage(IValidation validation, INewUsersStorage origin)
    : INewUsersStorage
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
