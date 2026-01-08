using System.Net;

namespace WebHostApplication.Common.Envelope;

public static class EnvelopeExtensions
{
    public static RemTech.SharedKernel.Web.Envelope NotFoundOrOk<T>(this T? result, string onNotFound) where T : class =>
        result is null 
            ? new RemTech.SharedKernel.Web.Envelope((int)HttpStatusCode.NotFound, null, onNotFound) 
            : new RemTech.SharedKernel.Web.Envelope((int)HttpStatusCode.OK, result, null);
}