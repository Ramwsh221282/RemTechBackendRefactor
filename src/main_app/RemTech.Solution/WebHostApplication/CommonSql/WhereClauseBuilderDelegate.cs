using Dapper;

namespace WebHostApplication.CommonSql;

/// <summary>
/// Контракт для построения WHERE части SQL запроса
/// </summary>
/// <typeparam name="T">Источник частей фильтров (например id = @id, name = @name и т.д.)</typeparam>
/// <param name="source">Источник частей фильтров.</param>
/// <param name="parameters">Параметры Dapper для добавления параметров в запрос.</param>
/// <returns>Строка с построенной WHERE частью SQL запроса.</returns>
public delegate string WhereClauseBuilderDelegate<T>(T source, DynamicParameters parameters);

/// <summary>
/// Просто класс пустышка, чтобы по нему обращались к extension методам.
/// </summary>
public static class SqlBuilderDelegate;

/// <summary>
/// Расширения для <see cref="SqlBuilderDelegate"/>
/// </summary>
public static class SqlBuilderDelegateImplementation
{
	extension(SqlBuilderDelegate)
	{
		public static string CombineWhereClauses<T>(
			T source,
			DynamicParameters parameters,
			params WhereClauseBuilderDelegate<T>[] parts
		)
		{
			string[] filterParts =
			[
				.. parts.Select(p => p.Invoke(source, parameters).Trim()).Where(s => !string.IsNullOrWhiteSpace(s)),
			];

			return filterParts.Length == 0 ? string.Empty : "WHERE " + string.Join(" AND ", filterParts);
		}
	}
}
