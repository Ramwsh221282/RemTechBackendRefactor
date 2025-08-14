using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Shared.Infrastructure.Module.Json;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Models.Features.CreatingNewAccount;
using Users.Module.Models.Features.VerifyingAdmin;

namespace Users.Module.Models;

internal sealed class UserJwt
{
    private readonly Guid _userId;
    private readonly Guid _tokenId;
    private readonly string _name;
    private readonly string _password;
    private readonly string _email;
    private readonly string _role;
    private readonly string _token;
    private readonly string _refreshToken;

    public async Task<UserJwt> CheckSubscription(
        SecurityKeySource key,
        ConnectionMultiplexer multiplexer
    )
    {
        if (!await key.IsTokenValid(_refreshToken))
        {
            await Deleted(multiplexer);
            throw new TokensExpiredException();
        }
        if (!await key.IsTokenValid(_token))
            return Recreate(key);

        return this;
    }

    public UserJwt Recreate(SecurityKeySource key)
    {
        UserJwtSource source = new(_userId, _name, _email, _password, _role);
        return source.Provide(key, _tokenId);
    }

    public async Task<UserJwt> Deleted(ConnectionMultiplexer multiplexer)
    {
        IDatabase database = multiplexer.GetDatabase();
        await database.KeyDeleteAsync(Key());
        return this;
    }

    public async Task StoreInCache(ConnectionMultiplexer multiplexer)
    {
        string value = new JsonObjectSnapshot()
            .With(nameof(_userId), _userId)
            .With(nameof(_tokenId), _tokenId)
            .With(nameof(_name), _name)
            .With(nameof(_password), _password)
            .With(nameof(_email), _email)
            .With(nameof(_role), _role)
            .With(nameof(_token), _token)
            .With(nameof(_refreshToken), _refreshToken)
            .Read();
        IDatabase database = multiplexer.GetDatabase();
        await database.StringSetAsync(Key(), value, TimeSpan.FromDays(7));
    }

    public string Key() => $"user_jwt_{_tokenId.ToString()}";

    public bool IsOfRole(string role)
    {
        return _role == role;
    }

    public async Task<UserJwt> Provide(ConnectionMultiplexer multiplexer)
    {
        IDatabase database = multiplexer.GetDatabase();
        string? value = await database.StringGetAsync(Key());
        if (string.IsNullOrEmpty(value))
            throw new UnableToGetUserJwtValueException();
        using JsonDocument document = JsonDocument.Parse(value);
        Guid tokenId = document.RootElement.GetProperty(nameof(_tokenId)).GetGuid();
        if (_tokenId != tokenId)
            throw new UserJwtTokenComparisonDifferentException();
        Guid userId = document.RootElement.GetProperty(nameof(_userId)).GetGuid();
        string name = document.RootElement.GetProperty(nameof(_name)).GetString()!;
        string password = document.RootElement.GetProperty(nameof(_password)).GetString()!;
        string email = document.RootElement.GetProperty(nameof(_email)).GetString()!;
        string role = document.RootElement.GetProperty(nameof(_role)).GetString()!;
        string token = document.RootElement.GetProperty(nameof(_token)).GetString()!;
        string refreshToken = document.RootElement.GetProperty(nameof(_refreshToken)).GetString()!;
        return new UserJwt(userId, tokenId, name, password, email, role, token, refreshToken);
    }

    public JwtUserResult AsResult()
    {
        return new JwtUserResult(_tokenId.ToString(), _token);
    }

    public UserJwt()
    {
        _userId = Guid.Empty;
        _tokenId = Guid.Empty;
        _name = string.Empty;
        _password = string.Empty;
        _email = string.Empty;
        _role = string.Empty;
        _token = string.Empty;
        _refreshToken = string.Empty;
    }

    public UserJwt(Guid tokenId)
    {
        _tokenId = tokenId;
        _token = string.Empty;
        _name = string.Empty;
        _password = string.Empty;
        _email = string.Empty;
        _role = string.Empty;
        _token = string.Empty;
        _refreshToken = string.Empty;
    }

    public UserJwt(
        Guid userId,
        Guid tokenId,
        string name,
        string password,
        string email,
        string role,
        string token,
        string refreshToken
    )
    {
        _userId = userId;
        _tokenId = tokenId;
        _name = name;
        _password = password;
        _email = email;
        _role = role;
        _token = token;
        _refreshToken = refreshToken;
    }
}
