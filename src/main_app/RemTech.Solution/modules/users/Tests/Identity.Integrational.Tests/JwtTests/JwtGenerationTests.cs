using System.Security.Claims;
using System.Text.Json;
using Identity.Adapter.Jwt.Claims;
using Identity.Adapter.Jwt.Security;
using Identity.Domain.Sessions;
using Identity.Domain.Sessions.Ports;
using Identity.Domain.Users.Aggregate;
using Identity.Integrational.Tests.Common;
using Identity.Integrational.Tests.Common.Cases;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RemTech.Shared.Configuration.Options;

namespace Identity.Integrational.Tests.JwtTests;

public sealed class JwtGenerationTests(IdentityTestApplicationFactory factory)
    : BaseIdentityModuleTestClass(factory)
{
    private UserCaseResult _user = null!;

    [Fact]
    private async Task Generate_Jwt_Token()
    {
        var user = await UseCases.GetUserByEmailUserCase(_user.Email);
        Assert.NotNull(user);
        await UseCases.MakeAuthorizedSession(user);
    }

    [Fact]
    private async Task Refresh_Jwt_Token()
    {
        var user = await UseCases.GetUserByEmailUserCase(_user.Email);
        Assert.NotNull(user);
        var session = await UseCases.MakeAuthorizedSession(user);
        await UseCases.RefreshUserSession(session);
    }

    [Fact]
    private async Task Newly_created_token_is_valid()
    {
        var user = await UseCases.GetUserByEmailUserCase(_user.Email);
        Assert.NotNull(user);
        var created = await UseCases.MakeAuthorizedSession(user);
        bool valid = await UseCases.ValidateSessionToken(created);
        Assert.True(valid);
    }

    [Fact]
    private async Task Several_newly_tokens_are_valid()
    {
        for (int i = 0; i < 5; i++)
        {
            var user = await UseCases.GetUserByEmailUserCase(_user.Email);
            Assert.NotNull(user);
            var created = await UseCases.MakeAuthorizedSession(user);
            bool valid = await UseCases.ValidateSessionToken(created);
            Assert.True(valid);
        }
    }

    [Fact]
    private async Task Expired_token_is_invalid()
    {
        int seconds = 5;
        var user = await UseCases.GetUserByEmailUserCase(_user.Email);
        Assert.NotNull(user);
        var created = await UseCases.CreateFakeSession(user, seconds);
        await Task.Delay(TimeSpan.FromSeconds(seconds * 2));
        bool valid = await UseCases.ValidateSessionToken(created);
        Assert.False(valid);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var user = await CreateUser("userLogin", "userEmail@mail.com", "userPassword!23");
        _user = user;
    }

    private async Task<UserCaseResult> CreateUser(string login, string email, string password) =>
        await new UserCreationCase("userLogin", "userEmail@mail.com", "userPassword!23").Invoke(
            this
        );
}

public sealed class FakeJwtSessionTokensMaker(
    IJwtTokenClaimsFactory claimsFactory,
    IRsaSecurityTokenPairStorage keysStorage,
    IOptions<FrontendOptions> frontend,
    int secondsTillExpire
) : IUserSessionMaker
{
    private readonly JsonWebTokenHandler _handler = new();

    public async Task<UserSession> Create(User user)
    {
        await keysStorage.Generate();

        var key = await keysStorage.Get();
        var claims = CreateClaims(user);
        var descriptor = CreateDescriptor(key, claims);

        return new UserSession(CreateJwtToken(descriptor), CreateRefreshTokenInfo());
    }

    private IEnumerable<Claim> CreateClaims(User user)
    {
        return claimsFactory.Create(user);
    }

    private SecurityTokenDescriptor CreateDescriptor(RsaSecurityKey key, IEnumerable<Claim> claims)
    {
        return new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new RsaSecurityKey(key.Rsa),
                SecurityAlgorithms.RsaSha256
            ),
            Issuer = "rem-tech-agg",
            Expires = DateTime.UtcNow.AddSeconds(secondsTillExpire),
            Audience = frontend.Value.FrontendUrl,
        };
    }

    private UserSessionInfo CreateJwtToken(SecurityTokenDescriptor descriptor)
    {
        string token = _handler.CreateToken(descriptor);
        return new UserSessionInfo(token);
    }

    private UserSessionInfo CreateRefreshTokenInfo()
    {
        DateTime expires = DateTime.UtcNow.AddHours(12);
        string token = Guid.NewGuid().ToString();
        string type = "refresh_token";

        var structure = new { type, details = new { token, expires } };
        string content = JsonSerializer.Serialize(structure);

        return new UserSessionInfo(content);
    }
}
