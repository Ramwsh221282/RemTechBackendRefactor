using System.Data;
using System.Runtime.CompilerServices;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Расширения для IDataReader.
/// </summary>
public static class DbDataReaderExtensions
{
	/// <summary>
	/// Получает значение указанного типа из столбца с заданным именем.
	/// </summary>
	/// <typeparam name="T">Тип значения для получения.</typeparam>
	/// <param name="reader">Объект IDataReader для чтения данных.</param>
	/// <param name="columnName">Имя столбца для получения значения.</param>
	/// <returns>Значение указанного типа из столбца.</returns>
	/// <exception cref="NotSupportedException">Если тип не поддерживается.</exception>
	public static T GetValue<T>(this IDataReader reader, string columnName)
	{
		Type requiredType = typeof(T);
		return requiredType switch
		{
			{ } t when t == typeof(bool) => GetBoolean<T>(reader, columnName),
			{ } t when t == typeof(int) => GetInt32<T>(reader, columnName),
			{ } t when t == typeof(long) => GetInt64<T>(reader, columnName),
			{ } t when t == typeof(Guid) => GetGuid<T>(reader, columnName),
			{ } t when t == typeof(string) => GetString<T>(reader, columnName),
			{ } t when t == typeof(DateTime) => GetDateTime<T>(reader, columnName),
			_ => throw new NotSupportedException($"Unsupported type {requiredType.Name}"),
		};
	}

	/// <summary>
	/// Проверяет, является ли значение в указанном столбце NULL.
	/// </summary>
	/// <param name="reader">Объект IDataReader для чтения данных.</param>
	/// <param name="columnName">Имя столбца для проверки значения на NULL.</param>
	/// <returns>True, если значение в столбце является NULL, иначе false.</returns>
	public static bool IsNull(this IDataReader reader, string columnName)
	{
		return reader.IsDBNull(reader.GetOrdinal(columnName));
	}

	/// <summary>
	/// Получает значение указанного типа из столбца с заданным именем, возвращая null, если значение является NULL в базе данных.
	/// </summary>
	/// <typeparam name="T">Тип значения для получения.</typeparam>
	/// <param name="reader">Объект IDataReader для чтения данных.</param>
	/// <param name="columnName">Имя столбца для получения значения.</param>
	/// <returns>Значение указанного типа из столбца или null, если значение является NULL в базе данных.</returns>
	/// <exception cref="NotSupportedException">Если тип не поддерживается.</exception>
	public static T? GetNullable<T>(this IDataReader reader, string columnName)
		where T : struct
	{
		Type requiredType = typeof(T);
		return requiredType switch
		{
			{ } t when t == typeof(bool) => reader.IsDBNull(reader.GetOrdinal(columnName))
				? null
				: GetBoolean<T>(reader, columnName),
			{ } t when t == typeof(int) => reader.IsDBNull(reader.GetOrdinal(columnName))
				? null
				: GetInt32<T>(reader, columnName),
			{ } t when t == typeof(long) => reader.IsDBNull(reader.GetOrdinal(columnName))
				? null
				: GetInt64<T>(reader, columnName),
			{ } t when t == typeof(Guid) => reader.IsDBNull(reader.GetOrdinal(columnName))
				? null
				: GetGuid<T>(reader, columnName),
			{ } t when t == typeof(DateTime) => reader.IsDBNull(reader.GetOrdinal(columnName))
				? null
				: GetDateTime<T>(reader, columnName),
			_ => throw new NotSupportedException($"Unsupported type {requiredType.Name}"),
		};
	}

	/// <summary>
	/// Получает значение указанного типа из столбца с заданным именем, возвращая null, если значение является NULL в базе данных.
	/// </summary>
	/// <typeparam name="T">Тип значения для получения.</typeparam>
	/// <param name="reader">Объект IDataReader для чтения данных.</param>
	/// <param name="columnName">Имя столбца для получения значения.</param>
	/// <returns>Значение указанного типа из столбца или null, если значение является NULL в базе данных.</returns>
	/// <exception cref="NotSupportedException">Если тип не поддерживается.</exception>
	public static T? GetNullableReferenceType<T>(this IDataReader reader, string columnName)
		where T : class
	{
		Type requiredType = typeof(T);
		return requiredType switch
		{
			{ } t when t == typeof(string) => reader.IsDBNull(reader.GetOrdinal(columnName))
				? null
				: GetString<T>(reader, columnName),
			_ => throw new NotSupportedException($"Unsupported type {requiredType.Name}"),
		};
	}

	private static T GetBoolean<T>(IDataReader reader, string columnName)
	{
		bool value = reader.GetBoolean(reader.GetOrdinal(columnName));
		return Unsafe.As<bool, T>(ref value);
	}

	private static T GetInt32<T>(IDataReader reader, string columnName)
	{
		int value = reader.GetInt32(reader.GetOrdinal(columnName));
		return Unsafe.As<int, T>(ref value);
	}

	private static T GetGuid<T>(IDataReader reader, string columnName)
	{
		Guid value = reader.GetGuid(reader.GetOrdinal(columnName));
		return Unsafe.As<Guid, T>(ref value);
	}

	private static T GetInt64<T>(IDataReader reader, string columnName)
	{
		long value = reader.GetInt64(reader.GetOrdinal(columnName));
		return Unsafe.As<long, T>(ref value);
	}

	private static T GetDateTime<T>(IDataReader reader, string columnName)
	{
		DateTime value = reader.GetDateTime(reader.GetOrdinal(columnName));
		return Unsafe.As<DateTime, T>(ref value);
	}

	private static T GetString<T>(IDataReader reader, string columnName)
	{
		string value = reader.GetString(reader.GetOrdinal(columnName));
		return Unsafe.As<string, T>(ref value);
	}
}
