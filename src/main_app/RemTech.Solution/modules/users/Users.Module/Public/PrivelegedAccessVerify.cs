using Npgsql;
using Users.Module.Models;
using Users.Module.Models.Features.AuthenticatingUserAccount;

namespace Users.Module.Public;

public sealed class PrivelegedAccessVerify(NpgsqlDataSource dataSource)
{
    public async Task<bool> AuthenticateByEmail(string email, string password)
    {
        IUserAuthentication authentication = new User(
            null,
            password,
            email
        ).RequireAuthentication();
        return await Authenticated(authentication);
    }

    public async Task<bool> AuthenticateByUsername(string username, string password)
    {
        IUserAuthentication authentication = new User(
            username,
            password,
            null
        ).RequireAuthentication();
        return await Authenticated(authentication);
    }

    private async Task<bool> Authenticated(IUserAuthentication authentication)
    {
        try
        {
            await authentication.Authenticate(dataSource);
            return true;
        }
        catch (AuthenticationEmailNotFoundException)
        {
            return false;
        }
        catch (AuthenticationPasswordFailedException)
        {
            return false;
        }
        catch (AuthenticationPasswordNotProvidedException)
        {
            return false;
        }
        catch (AuthenticationUserNameNotFoundException)
        {
            return false;
        }
        catch (UnableToResolveAuthenticationMethodException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
