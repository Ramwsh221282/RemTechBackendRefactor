namespace RemTech.SharedKernel.Core.PrimitivesModule;

/// <summary>
/// Строка электронной почты.
/// </summary>
public sealed record EmailString
{
	private readonly bool _hasValidFormat;

	/// <summary>
	/// Инициализирует новый экземпляр <see cref="EmailString"/>.
	/// </summary>
	/// <param name="value">Значение строки электронной почты.</param>
	/// <param name="hasValidFormat">Флаг, указывающий, имеет ли строка допустимый формат электронной почты.</param>
	internal EmailString(string value, bool hasValidFormat)
	{
		Value = value;
		_hasValidFormat = hasValidFormat;
	}

	/// <summary>
	/// Значение строки электронной почты.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Проверяет, имеет ли строка допустимый формат электронной почты.
	/// </summary>
	/// <returns>True, если строка имеет допустимый формат электронной почты; иначе false.</returns>
	public bool IsValid()
	{
		return _hasValidFormat;
	}
}
