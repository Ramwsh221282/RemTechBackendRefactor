using System.Security.Claims;
using Identity.Domain.Users.Aggregate;

namespace Identity.Adapter.Jwt.Claims;

public interface IJwtTokenClaimsFactory
{
    IEnumerable<Claim> Create(User user);
}
