namespace Users.Module.Features.RegisteringUser.Storage;

internal interface IUsersStorage
{
    Task<bool> Save(string name, string email, string password, CancellationToken ct = default);
}
