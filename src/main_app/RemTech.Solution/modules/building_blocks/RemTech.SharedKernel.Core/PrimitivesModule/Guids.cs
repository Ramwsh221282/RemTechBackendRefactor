namespace RemTech.SharedKernel.Core.PrimitivesModule;

/// <summary>
/// Утилиты для работы с GUID.
/// </summary>
public static class Guids
{
	/// <summary>
	/// Проверяет, что GUID не является пустым.
	/// </summary>
	/// <param name="id">GUID для проверки.</param>
	/// <returns>True, если GUID не пустой; иначе false.</returns>
	public static bool NotEmpty(Guid? id) => id.HasValue;

	/// <summary>
	/// Проверяет, что GUID не является пустым.
	/// </summary>
	/// <param name="id">GUID для проверки.</param>
	/// <returns>True, если GUID не пустой; иначе false.</returns>
	public static bool NotEmpty(Guid id) => id != Guid.Empty;

	/// <summary>
	/// Проверяет, что GUID является пустым.
	/// </summary>
	/// <param name="id">GUID для проверки.</param>
	/// <returns>True, если GUID пустой; иначе false.</returns>
	public static bool Empty(Guid id) => id == Guid.Empty;

	/// <summary>
	/// Проверяет, что GUID является пустым.
	/// </summary>
	/// <param name="id">GUID для проверки.</param>
	/// <returns>True, если GUID пустой; иначе false.</returns>
	public static bool Empty(Guid? id) => !id.HasValue || id.Value == Guid.Empty;
}
