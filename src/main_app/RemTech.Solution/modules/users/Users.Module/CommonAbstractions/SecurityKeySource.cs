using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Users.Module.CommonAbstractions;

internal sealed class SecurityKeySource
{
    private readonly byte[] _securityKeyBytes;

    public SecurityKeySource()
    {
        _securityKeyBytes = Encoding.UTF8.GetBytes(GenerateSecurityKeyFromUUID());
    }

    public SigningCredentials Credentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(_securityKeyBytes),
            SecurityAlgorithms.HmacSha256
        );
    }

    private static string GenerateSecurityKeyFromUUID()
    {
        Guid uuid = Guid.NewGuid();
        string uuidString = uuid.ToString();
        byte[] bytes = Encoding.UTF8.GetBytes(uuidString);
        string base64 = Convert.ToBase64String(bytes);
        return base64;
    }
}
