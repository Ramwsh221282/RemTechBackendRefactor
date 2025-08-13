using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Users.Module.CommonAbstractions;

public sealed class SecurityKeySource
{
    private readonly byte[] _securityKeyBytes;
    private TokenValidationParameters? _validationParameters;

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

    public async Task<bool> IsTokenValid(string token)
    {
        _validationParameters ??= ValidationParameters();
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            TokenValidationResult validation = await tokenHandler.ValidateTokenAsync(
                token,
                _validationParameters
            );
            return validation.IsValid;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static string GenerateSecurityKeyFromUUID()
    {
        Guid uuid = Guid.NewGuid();
        string uuidString = uuid.ToString();
        byte[] bytes = Encoding.UTF8.GetBytes(uuidString);
        string base64 = Convert.ToBase64String(bytes);
        return base64;
    }

    private TokenValidationParameters ValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = Credentials().Key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    }
}
