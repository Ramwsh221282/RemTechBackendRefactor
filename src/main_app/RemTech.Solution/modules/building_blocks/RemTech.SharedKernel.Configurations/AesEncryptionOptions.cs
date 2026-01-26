using System.Text;

namespace RemTech.SharedKernel.Configurations;

public class AesEncryptionOptions
{
	public string PlainKey { get; set; } = string.Empty;

	public byte[] KeyAsBytes()
	{
		ValidatePlainKey();
		return StringAs16LengthByteArray(PlainKey);
	}

	public byte[] IV4AsBytes()
	{
		ValidatePlainKey();
		string combinedGuids = $"{Guid.NewGuid()}{Guid.NewGuid()}";
		return StringAs16LengthByteArray(combinedGuids);
	}

	private static byte[] StringAs16LengthByteArray(string inputText)
	{
		Span<byte> encoded = new(Encoding.UTF8.GetBytes(inputText));
		Span<byte> cut = encoded[..16];
		return cut.ToArray();
	}

	private void ValidatePlainKey()
	{
		if (string.IsNullOrWhiteSpace(PlainKey))
			throw new ArgumentException("Cannot use Aes Encryption Options. Plain key was not set.");
	}
}
