using Mailing.Module.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mailing.Module.Features;

public static class UpdateMailingSender
{
    public sealed record UpdateMailingSenderRequest(string Name, string Email, string Password);

    public sealed record UpdateMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapPatch(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromBody] UpdateMailingSenderRequest request,
        [FromServices] IEmailSendersSource source,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        EmailSenderOutput output = await Process(source, request, ct);
        UpdateMailingSenderResponse response = new(output.Name, output.Email);
        logger.Information("Updated mailing sender. {Name} - {Email}.", output.Name, output.Email);
        return Results.Ok(response);
    }

    private static async Task<EmailSenderOutput> Process(
        IEmailSendersSource source,
        UpdateMailingSenderRequest request,
        CancellationToken ct = default
    )
    {
        IEmailSender sender = await source.Get(request.Name, ct);
        sender = sender.ChangeEmail(request.Email).ChangePassword(request.Password);
        await sender.Update(source, ct);
        return sender.Print();
    }
}
