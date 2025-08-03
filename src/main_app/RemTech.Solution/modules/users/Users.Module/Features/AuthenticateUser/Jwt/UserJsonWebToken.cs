using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.AuthenticateUser.Jwt;

internal sealed class UserJsonWebToken
{
    private readonly SecurityKeySource _keySource;
    private readonly Guid _tokenId;
    private Guid _userId;
    private DateTime _expiredAt;
    private string _refreshToken;

    public UserJsonWebToken(SecurityKeySource securityKey)
    {
        _keySource = securityKey;
        _tokenId = Guid.NewGuid();
        _userId = Guid.Empty;
        _expiredAt = DateTime.MinValue;
        _refreshToken = string.Empty;
    }

    public UserJsonWebToken(SecurityKeySource securityKey, Guid userId, string tokenId)
        : this(securityKey)
    {
        _userId = userId;
        _tokenId = Guid.Parse(tokenId);
    }

    public UserJsonWebToken WithUserId(Guid id)
    {
        _userId = id;
        return this;
    }

    public async Task Save(IJsonWebTokensStorage storage, CancellationToken ct = default)
    {
        if (_userId == Guid.Empty)
            throw new ApplicationException("User id was not provided.");
        GenerateRefreshToken();
        JsonWebTokenClaimsDataOutput output = new(_tokenId, _refreshToken, _userId);
        await storage.SaveTokenClaimsData(output, ct);
    }

    public string TokenString()
    {
        if (_userId == Guid.Empty)
            throw new ApplicationException("User id was not provided.");
        GenerateRefreshToken();
        _expiredAt = DateTime.Now.AddMinutes(3);
        Claim[] claims =
        [
            new("userId", _userId.ToString()),
            new("tokenId", Guid.NewGuid().ToString()),
        ];
        SigningCredentials credentials = _keySource.Credentials();
        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: credentials,
            expires: _expiredAt
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void GenerateRefreshToken()
    {
        if (string.IsNullOrWhiteSpace(_refreshToken))
            _refreshToken = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())
            );
    }
}
