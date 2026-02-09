using System.Text;

namespace RemTech.SharedKernel.Configurations;

/// <summary>
/// Конфигурация для AES шифрования.
/// </summary>
public class AesEncryptionOptions
{
	/// <summary>
	/// Значение ключа шифрования в открытом виде.
	/// </summary>
	public string PlainKey { get; set; } = string.Empty;

	/// <summary>
	/// Получить ключ шифрования в виде массива байт длиной 16.
	/// </summary>
	/// <returns>Массив байт длиной 16, представляющий ключ шифрования.</returns>
	public byte[] KeyAsBytes()
	{
		ValidatePlainKey();
		return StringAs16LengthByteArray(PlainKey);
	}

	/// <summary>
	/// Получить вектор инициализации (IV) в виде массива байт длиной 16.
	/// </summary>
	/// <returns>Массив байт длиной 16, представляющий вектор инициализации (IV).</returns>
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
		{
			throw new ArgumentException("Cannot use Aes Encryption Options. Plain key was not set.");
		}
	}
}
