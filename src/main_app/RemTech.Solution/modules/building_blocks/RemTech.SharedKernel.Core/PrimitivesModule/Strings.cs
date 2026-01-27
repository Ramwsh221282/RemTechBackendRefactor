namespace RemTech.SharedKernel.Core.PrimitivesModule;

/// <summary>
/// Утилиты для работы со строками.
/// </summary>
public static class Strings
{
	extension(string[] source)
	{
		public string Join(char separator) => Joined(source, separator);

		public string Join(string separator) => Joined(source, separator);
	}

	/// <summary>
	/// Объединяет элементы массива строк с указанным разделителем.
	/// </summary>
	/// <param name="source">Массив строк для объединения.</param>
	/// <param name="separator">Разделитель для объединения.</param>
	/// <returns>Объединенная строка.</returns>
	public static string Joined(string[] source, char separator) => string.Join(separator, source);

	/// <summary>
	/// Объединяет элементы перечисления строк с указанным разделителем.
	/// </summary>
	/// <param name="source">Перечисление строк для объединения.</param>
	/// <param name="separator">Разделитель для объединения.</param>
	/// <returns>Объединенная строка.</returns>
	public static string Joined(IEnumerable<string> source, char separator) => string.Join(separator, source);

	/// <summary>
	/// Объединяет элементы перечисления строк с указанным разделителем.
	/// </summary>
	/// <param name="source">Перечисление строк для объединения.</param>
	/// <param name="separator">Разделитель для объединения.</param>
	/// <returns>Объединенная строка.</returns>
	public static string Joined(IEnumerable<string> source, string separator) => string.Join(separator, source);

	/// <summary>
	/// Объединяет элементы массива строк с указанным разделителем.
	/// </summary>
	/// <param name="source">Массив строк для объединения.</param>
	/// <param name="separator">Разделитель для объединения.</param>
	/// <returns>Объединенная строка.</returns>
	public static string Joined(string[] source, string separator) => string.Join(separator, source);

	/// <summary>
	/// Проверяет, является ли строка пустой или содержит только пробельные символы.
	/// </summary>
	/// <param name="str">Строка для проверки.</param>
	/// <returns>True, если строка пустая или содержит только пробельные символы; иначе false.</returns>
	public static bool EmptyOrWhiteSpace(string? str) => string.IsNullOrWhiteSpace(str);

	/// <summary>
	/// Проверяет, что строка не является пустой и не содержит только пробельные символы.
	/// </summary>
	/// <param name="str">Строка для проверки.</param>
	/// <returns>True, если строка не пустая и не содержит только пробельные символы; иначе false.</returns>
	public static bool NotEmptyOrWhiteSpace(string? str)
	{
		bool empty = EmptyOrWhiteSpace(str);
		return !empty;
	}

	/// <summary>
	/// Проверяет, что длина строки больше указанного значения.
	/// </summary>
	/// <param name="str">Строка для проверки.</param>
	/// <param name="length">Значение длины для сравнения.</param>
	/// <returns>True, если длина строки больше указанного значения; иначе false.</returns>
	public static bool GreaterThan(string str, int length) => str.Length > length;

	/// <summary>
	/// Проверяет, что длина строки не больше указанного значения.
	/// </summary>
	/// <param name="str">Строка для проверки.</param>
	/// <param name="length">Значение длины для сравнения.</param>
	/// <returns>True, если длина строки не больше указанного значения; иначе false.</returns>
	public static bool NotGreaterThan(string str, int length)
	{
		bool greater = GreaterThan(str, length);
		return !greater;
	}

	/// <summary>
	/// Проверяет, что длина строки меньше указанного значения.
	/// </summary>
	/// <param name="str">Строка для проверки.</param>
	/// <param name="length">Значение длины для сравнения.</param>
	/// <returns>True, если длина строки меньше указанного значения; иначе false.</returns>
	public static bool LesserThan(string str, int length) => str.Length < length;

	/// <summary>
	/// Проверяет, что длина строки не меньше указанного значения.
	/// </summary>
	/// <param name="str">Строка для проверки.</param>
	/// <param name="length">Значение длины для сравнения.</param>
	/// <returns>True, если длина строки не меньше указанного значения; иначе false.</returns>
	public static bool NotLesserThan(string str, int length) => !LesserThan(str, length);
}
