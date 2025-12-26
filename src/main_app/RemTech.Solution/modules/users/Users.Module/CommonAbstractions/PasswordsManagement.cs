using Microsoft.Extensions.DependencyInjection;

namespace Users.Module.CommonAbstractions;

internal delegate string PasswordHashing(string input);

internal delegate bool PasswordVerification(string plainPassword, string hashedPassword);

internal sealed record PasswordManagementTools(
    PasswordHashing PasswordHashing,
    PasswordVerification Verification
);

internal static class PasswordsManagement
{
    public static void AddPasswordManagementTools(this IServiceCollection services)
    {
        services.AddSingleton<StringHash>();
        services.AddSingleton<PasswordHashing>(sp =>
        {
            var hash = sp.GetRequiredService<StringHash>();
            return input => hash.Hash(input);
        });

        services.AddSingleton<PasswordVerification>(_ =>
            (plain, hashed) => BCrypt.Net.BCrypt.Verify(plain, hashed)
        );

        services.AddSingleton<PasswordManagementTools>(sp => new PasswordManagementTools(
            sp.GetRequiredService<PasswordHashing>(),
            sp.GetRequiredService<PasswordVerification>()
        ));
    }
}
