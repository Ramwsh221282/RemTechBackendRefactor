using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Entities.Profile.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.Common;

public sealed class MultiWayGetUserHandle(
    IGetUserByLoginHandle byLogin,
    IGetUserByEmailHandle byEmail
) : IGetUserHandle
{
    public async Task<Status<User>> Get(
        string? login,
        string? email,
        CancellationToken ct = default
    ) =>
        login switch
        {
            not null when CanLoginByLogin(login) => await byLogin.Handle(login, ct),
            null => email switch
            {
                null => Error.NotFound("Пользователь не найден."),
                not null when CanLoginByEmail(email) => await byEmail.Handle(email, ct),
                _ => Error.NotFound("Пользователь не найден."),
            },
            _ => Error.NotFound("Пользователь не найден."),
        };

    private bool CanLoginByLogin(string? login) =>
        UserLogin.Create(login ?? string.Empty).IsSuccess switch
        {
            true => true,
            false => false,
        };

    private bool CanLoginByEmail(string? email) =>
        UserEmail.Create(email ?? string.Empty).IsSuccess switch
        {
            true => true,
            false => false,
        };
}
