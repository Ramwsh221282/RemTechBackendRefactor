using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.AesEncryption;

public sealed record AesCryptography(IOptions<AesEncryptionOptions> Options)
{
	private const int BytesLength = 16;

	public async Task<string> EncryptText(string text, CancellationToken ct)
	{
		using Aes aes = Aes.Create();
		aes.Key = Options.Value.KeyAsBytes();
		aes.IV = Options.Value.IV4AsBytes();
		byte[] bytesAfterEncryption;
		await using (MemoryStream ms = new())
		{
			await ms.WriteAsync(aes.IV, 0, aes.IV.Length, ct);
			await using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
			await using (StreamWriter sw = new(cs, leaveOpen: false))
			{
				await sw.WriteAsync(text);
			}
			bytesAfterEncryption = ms.ToArray();
		}

		string encryptedText = Convert.ToBase64String(bytesAfterEncryption);
		return encryptedText;
	}

	public async Task<string> DecryptText(string text, CancellationToken ct)
	{
		byte[] base64Text = Convert.FromBase64String(text);
		byte[] iv = new byte[BytesLength];
		Array.Copy(base64Text, iv, BytesLength);
		using Aes aes = Aes.Create();
		aes.Key = Options.Value.KeyAsBytes();
		aes.IV = iv;
		using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
		await using MemoryStream ms = new(base64Text, BytesLength, base64Text.Length - BytesLength);
		await using CryptoStream cs = new(ms, transform, CryptoStreamMode.Read);
		using StreamReader sr = new(cs);
		string decryptedText = await sr.ReadToEndAsync(ct);
		return decryptedText;
	}
}
