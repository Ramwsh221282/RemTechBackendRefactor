using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using Identity.Domain.Users.UseCases.Common;
using Identity.Domain.Users.UseCases.UserRegistration;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Adapter.Storage.Seeding;

public sealed class UserSeeding(
    ICommandHandler<UserRegistrationCommand, Status<User>> handler,
    IUserEmailUnique emailUnique,
    IUserLoginUnique loginUnique
) : IUserSeeding
{
    public async Task SeedUser(string email, string login, string password)
    {
        if (!await HasEmailUnique(email))
            return;

        if (!await HasUniqueLogin(login))
            return;

        var command = new UserRegistrationCommand(login, email, password);
        var status = await handler.Handle(command);
        if (status.IsFailure)
            throw new ApplicationException($"Unable to seed user. Error: {status.Error.ErrorText}");
    }

    private async Task<bool> HasUniqueLogin(string login)
    {
        var validLogin = UserLogin.Create(login);
        if (validLogin.IsFailure)
            return true;

        var unique = await loginUnique.Unique(validLogin);
        return unique.IsSuccess;
    }

    private async Task<bool> HasEmailUnique(string email)
    {
        var validEmail = UserEmail.Create(email);
        if (validEmail.IsFailure)
            return true;

        var unique = await emailUnique.Unique(validEmail);
        return unique.IsSuccess;
    }
}
