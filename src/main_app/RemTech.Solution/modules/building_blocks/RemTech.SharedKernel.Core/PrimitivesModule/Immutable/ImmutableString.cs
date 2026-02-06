namespace RemTech.SharedKernel.Core.PrimitivesModule.Immutable;

/// <summary>
/// Неизменяемая строка.
/// </summary>
/// <param name="value">Значение строки.</param>
public sealed class ImmutableString(string value)
{
	private readonly string _value = value;

	/// <summary>
	/// Неявное преобразование из строки в ImmutableString.
	/// </summary>
	/// <param name="value">Значение строки.</param>
	public static implicit operator string(ImmutableString value)
	{
		return value.Read();
	}

	/// <summary>
	/// Читает значение строки.
	/// </summary>
	/// <returns>Значение строки.</returns>
	public string Read()
	{
		return _value;
	}

	/// <summary>
	/// Преобразует ImmutableString в строку.
	/// </summary>
	/// <returns>Значение строки.</returns>
	public override string ToString()
	{
		return _value;
	}
}
