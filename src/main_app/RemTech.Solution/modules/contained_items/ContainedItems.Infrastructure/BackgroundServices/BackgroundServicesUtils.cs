using System.Security.Cryptography;
using System.Text;
using RemTech.SharedKernel.Core.Handlers;

namespace ContainedItems.Infrastructure.BackgroundServices;

internal static class BackgroundServicesUtils
{
    public static string CreateKey<TQuery>(TQuery query) where TQuery : IQuery
    {
        string hashedPayload = ToSha256Hash(query.ToString()!);
        string key = $"{typeof(TQuery).Name}:{hashedPayload}";
        return key;
    }
    
    private static string ToSha256Hash(string input)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new(bytes.Length * 2);

        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}