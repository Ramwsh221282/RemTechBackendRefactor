namespace Users.Module.CommonAbstractions;

internal sealed class StringHash(int workFactor = 12)
{
    public string Hash(string input) => BCrypt.Net.BCrypt.HashPassword(input, workFactor);
}
