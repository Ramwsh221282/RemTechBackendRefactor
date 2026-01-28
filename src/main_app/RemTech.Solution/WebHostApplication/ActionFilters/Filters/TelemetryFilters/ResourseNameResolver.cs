namespace WebHostApplication.ActionFilters.Filters.TelemetryFilters;

/// <summary>
/// Парсит из пути к ресурсу человекочитаемое имя ресурса из HttpContext.
/// </summary>
public static class ResourseNameResolver
{
	/// <summary>
	/// Разрешает человекочитаемое имя ресурса из HttpContext.
	/// </summary>
	/// <param name="context">Контекст HTTP запроса.</param>
	/// <returns>Человекочитаемое имя ресурса.</returns>
	public static string ResolveHumanizedResourceNameFromHttpContext(HttpContext context)
	{
		string path = context.Request.Path.Value;
		if (string.IsNullOrWhiteSpace(path))
			return "Неизвестный ресурс";

		return path switch
		{
			_ when Includes("main-page/statistics", path) =>
				"Просмотр аггрегированной статистики по технике и запчастям на главной странице.",
			_ when Includes("main-page/last-added", path) =>
				"Просмотр списка последних добавленных запчастей и техники на главной странице.",
			_ when Includes("commit-password-reset", path) && Includes("identity", path) =>
				"Подтверждение сброса пароля.",
			_ when IncludesAll(["reset-password", "identity"], path) => "Создание заявки на сброс пароля.",
			_ when IncludesAll(["auth", "identity"], path) => "Авторизация на основе входных данных.",
			_ when IncludesAll(["password", "identity"], path) => "Создание заявки на изменение пароля.",
			_ when IncludesAll(["logout", "identity"], path) => "Выход из учетной записи в системе.",
			_ when IncludesAll(["account", "identity"], path) => "Чтение информации об учетной записи (профиле).",
			_ when IncludesAll(["confirmation", "identity"], path) =>
				"Подтверждение заявки (сброс пароля, регистрация и др.).",
			_ when IncludesAll(["permissions", "identity"], path) => "Назначение разрешений для учетной записи.",
			_ when IncludesAll(["refresh", "identity"], path) => "Продление авторизованной сессии.",
			_ when IncludesAll(["sign-up", "identity"], path) => "Регистрация.",
			_ when IncludesAll(["verify", "identity"], path) => "Авторизация на основе сессии.",
			_ when IncludesAll(["test-message", "notifications"], path) =>
				"Тестирование отправки сообщений при помощи почтового сервиса.",
			_ when Includes("mailers", path) => IncludesWithMethodTypeDispatch(
				path,
				"mailers",
				context,
				onGet: () => "Просмотр списка почтовых рассылок.",
				onPost: () => "Создание новой конфигурации для почтовой рассылки.",
				onPut: () => "Редактирование конфигурации почтовой рассылки.",
				onDelete: () => "Удаление конфигурации почтовой рассылки.",
				onPatch: () => "Редактирование конфигурации почтовой рассылки."
			),
			_ when IncludesAll(["start", "parsers"], path) => "Запуск парсера.",
			_ when IncludesAll(["links", "parsers"], path) => DispatchByMethod(
				context,
				onGet: () => "Просмотр ссылок для парсера.",
				onPost: () => "Создание ссылки для парсера.",
				onPut: () => "Редактирование ссылки для парсера.",
				onDelete: () => "Удаление ссылки для парсера."
			),
			_ when IncludesAll(["links", "parsers", "activity"], path) =>
				"Изменение статуса активности ссылки парсера.",
			_ when IncludesAll(["wait-days", "parsers"], path) => "Изменение дней ожидания парсера.",
			_ when IncludesAll(["permantly-start", "parsers"], path) => "Немедленный запуск парсера.",
			_ when IncludesAll(["permantly-disable", "parsers"], path) => "Немедленное выключение парсера.",
			_ when IncludesAll(["disabled", "parsers"], path) => "Выключение парсера.",
			_ when Includes("parsers", path) => DispatchByMethod(context, onGet: () => "Просмотр списка парсеров."),
			_ when Includes("spares", path) => "Просмотр списка запчастей.",
			_ when Includes("spares/locations", path) => "Просмотр списка геолокаций запчастей.",
			_ when Includes("spares/types", path) => "Просмотр списка типов запчастей.",
			_ when Includes("brands", path) => "Просмотр списка брендов техники.",
			_ when Includes("categories", path) => "Просмотр списка категорий техники.",
			_ when Includes("characteristics", path) => "Просмотр списка характеристик техники.",
			_ when Includes("locations", path) => "Просмотр списка геолокаций техники.",
			_ when Includes("models", path) => "Просмотр списка моделей техники.",
			_ when Includes("vehicles", path) => "Просмотр списка техники.",
			_ => "Неизвестный ресурс",
		};
	}

	private static bool IncludesAll(string[] routes, string segment) => routes.All(r => Includes(r, segment));

	private static bool Includes(string route, string segment) =>
		segment.Contains(route, StringComparison.OrdinalIgnoreCase);

	private static string DispatchByMethod(
		HttpContext httpContext,
		Func<string>? onGet = null,
		Func<string>? onPost = null,
		Func<string>? onPut = null,
		Func<string>? onDelete = null,
		Func<string>? onPatch = null
	)
	{
		string method = httpContext.Request.Method;
		Func<string>? dispatchedFunction = method switch
		{
			"GET" => onGet,
			"POST" => onPost,
			"PUT" => onPut,
			"DELETE" => onDelete,
			"PATCH" => onPatch,
			_ => () => "Неизвестный ресурс.",
		};

		return dispatchedFunction?.Invoke() ?? "Неизвестный ресурс.";
	}

	private static string IncludesWithMethodTypeDispatch(
		string segment,
		Func<string, bool> includes,
		HttpContext httpContext,
		Func<string>? onGet = null,
		Func<string>? onPost = null,
		Func<string>? onPut = null,
		Func<string>? onDelete = null,
		Func<string>? onPatch = null
	)
	{
		bool contains = includes(segment);
		if (!contains)
			return "Неизвестный ресурс.";

		string method = httpContext.Request.Method;
		Func<string>? dispatchedFunction = method switch
		{
			"GET" => onGet,
			"POST" => onPost,
			"PUT" => onPut,
			"DELETE" => onDelete,
			"PATCH" => onPatch,
			_ => () => "Неизвестный ресурс.",
		};

		return dispatchedFunction?.Invoke() ?? "Неизвестный ресурс.";
	}

	private static string IncludesWithMethodTypeDispatch(
		string route,
		string segment,
		HttpContext httpContext,
		Func<string>? onGet = null,
		Func<string>? onPost = null,
		Func<string>? onPut = null,
		Func<string>? onDelete = null,
		Func<string>? onPatch = null
	)
	{
		bool contains = route.Contains(segment, StringComparison.OrdinalIgnoreCase);
		if (!contains)
			return "Неизвестный ресурс.";

		string method = httpContext.Request.Method;
		Func<string>? dispatchedFunction = method switch
		{
			"GET" => onGet,
			"POST" => onPost,
			"PUT" => onPut,
			"DELETE" => onDelete,
			"PATCH" => onPatch,
			_ => () => "Неизвестный ресурс.",
		};

		return dispatchedFunction?.Invoke() ?? "Неизвестный ресурс.";
	}
}
