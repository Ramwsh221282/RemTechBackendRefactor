using Microsoft.Extensions.Options;

namespace Identity.WebApi.Options;

public sealed class SuperUserCredentialsOptions
{
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Login))
            throw new InvalidOperationException("Super user login is empty.");
        if (string.IsNullOrWhiteSpace(Email))
            throw new InvalidOperationException("Super user email is empty.");
        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("Super user password is empty.");
    }

    public static void AddFromAppsettings(
        IServiceCollection services,
        string section = nameof(SuperUserCredentialsOptions)
    ) => services.AddOptions<IOptions<SuperUserCredentialsOptions>>().BindConfiguration(section);
}
