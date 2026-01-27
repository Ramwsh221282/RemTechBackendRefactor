using System.Net;

namespace WebHostApplication.Common.Envelope;

/// <summary>
/// Фабрика для создания экземпляров <see cref="RemTech.SharedKernel.Web.Envelope"/>.
/// </summary>
public static class EnvelopeFactory
{
	/// <summary>
	/// Возвращает ответ с кодом 404 Not Found, если результат равен null, иначе возвращает ответ с кодом 200 OK и результатом.
	/// </summary>
	/// <typeparam name="T"> Тип результата. </typeparam>
	/// <param name="result"> Результат операции. </param>
	/// <param name="onNotFound"> Сообщение, если результат не найден. </param>
	/// <returns> Экземпляр <see cref="RemTech.SharedKernel.Web.Envelope"/> с соответствующим статусом и данными. </returns>
	public static RemTech.SharedKernel.Web.Envelope NotFoundOrOk<T>(this T? result, string onNotFound)
		where T : class =>
		result is null
			? new RemTech.SharedKernel.Web.Envelope((int)HttpStatusCode.NotFound, null, onNotFound)
			: new RemTech.SharedKernel.Web.Envelope((int)HttpStatusCode.OK, result, null);

	/// <summary>
	/// Возвращает ответ с кодом 200 OK и результатом.
	/// </summary>
	/// <typeparam name="T"> Тип результата. </typeparam>
	/// <param name="result"> Результат операции. </param>
	/// <returns> Экземпляр <see cref="RemTech.SharedKernel.Web.Envelope"/> с кодом 200 OK и результатом. </returns>
	public static RemTech.SharedKernel.Web.Envelope Ok<T>(this T result)
		where T : notnull => new((int)HttpStatusCode.OK, result, null);

	/// <summary>
	/// Возвращает ответ с кодом 404 Not Found и сообщением.
	/// </summary>
	/// <param name="message"> Сообщение об ошибке. </param>
	/// <returns> Экземпляр <see cref="RemTech.SharedKernel.Web.Envelope"/> с кодом 404 Not Found и сообщением. </returns>
	public static RemTech.SharedKernel.Web.Envelope NotFound(string message) =>
		new((int)HttpStatusCode.NotFound, null, message);
}
