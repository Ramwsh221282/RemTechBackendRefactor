using System.Data;
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
/// Контракт для построения части SQL запроса для пагинации.
/// </summary>
/// <typeparam name="T">Источник данных для выбора страницы и размера страницы.</typeparam>
/// <param name="pageSelector">Функция для выбора номера страницы из источника данных.</param>
/// <param name="pageSizeSelector">Функция для выбора размера страницы из источника данных.</param>
/// <returns>Строка с построенной частью SQL запроса для пагинации.</returns>
public delegate string PaginationClauseBuilderDelegate<T>(Func<T, int> pageSelector, Func<T, int> pageSizeSelector);

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

		public static string BuildPaginationClause<T>(
			T source,
			DynamicParameters parameters,
			Func<T, int> pageSelector,
			Func<T, int> pageSizeSelector
		)
		{
			int pageSize = pageSizeSelector.Invoke(source);
			int page = pageSelector.Invoke(source);
			int limit = pageSize;
			int offset = (page - 1) * pageSize;
			parameters.Add("@PageSize", limit, DbType.Int32);
			parameters.Add("@Page", offset, DbType.Int32);
			return "LIMIT @PageSize OFFSET @Page";
		}
	}
}
