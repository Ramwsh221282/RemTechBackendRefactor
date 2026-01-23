using System.Net;

namespace WebHostApplication.Common.Envelope;

public static class EnvelopeFactory
{
	public static RemTech.SharedKernel.Web.Envelope NotFoundOrOk<T>(this T? result, string onNotFound)
		where T : class =>
		result is null
			? new RemTech.SharedKernel.Web.Envelope((int)HttpStatusCode.NotFound, null, onNotFound)
			: new RemTech.SharedKernel.Web.Envelope((int)HttpStatusCode.OK, result, null);

	public static RemTech.SharedKernel.Web.Envelope Ok<T>(this T result)
		where T : notnull => new((int)HttpStatusCode.OK, result, null);

	public static RemTech.SharedKernel.Web.Envelope NotFound(string message) =>
		new((int)HttpStatusCode.NotFound, null, message);
}
