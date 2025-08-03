using Users.Module.Features.AuthenticateUser.Jwt;

namespace Users.Module.Features.AuthenticateUser;

internal interface IExistingUser
{
    bool OwnsPassword(UserAuthenticationPassword password);
    UserJsonWebToken PrintToToken(UserJsonWebToken token);
}
