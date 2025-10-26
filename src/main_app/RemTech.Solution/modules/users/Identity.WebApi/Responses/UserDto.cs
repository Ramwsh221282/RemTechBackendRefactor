using Identity.Domain.Users.Aggregate;

namespace Identity.WebApi.Responses;

public sealed record UserDto
{
    public Guid Id { get; }
    public string Login { get; }
    public string Email { get; }
    public IEnumerable<string> Roles { get; }

    public UserDto(User user)
    {
        Id = user.Id.Id;
        Login = user.Profile.Login.Name;
        Email = user.Profile.Email.Email;
        Roles = user.Roles.Roles.Select(r => r.Name.Value);
    }
}
