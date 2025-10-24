using Identity.Domain.Users.Aggregate;

namespace Identity.Jwt.Port;

public interface ISessionUserSetup
{
    void Import();
}

public interface ISessionUserBaker
{
    Task<ISessionUserSetup> Bake(User user);
}

public interface ISessionUserSetupAsync
{
    Task Import();
}

public interface ISessionUser
{
    public void ImportClaims();
    public void ImportKey();
    public void ImportDescriptor();
    public void Create();

    // public IEnumerable<Claim> Claims { get; set; }
    // public RsaSecurityKey Key { get; set; }
    // public SecurityTokenDescriptor Descriptor { set; }
    // public IJwtUser Create();
    // public string ReadToken();
}
