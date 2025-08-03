using Mailing.Module.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mailing.Module.Features;

public static class RemoveMailingSender
{
    public sealed record RemoveMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapDelete(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromQuery] string email,
        [FromServices] IEmailSendersSource senders,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        EmailSenderOutput output = await Process(senders, email, ct);
        RemoveMailingSenderResponse response = new(output.Name, output.Email);
        logger.Information("Removed mailing sender {Name} - {Email}.", output.Name, output.Email);
        return Results.Ok(response);
    }

    private static async Task<EmailSenderOutput> Process(
        IEmailSendersSource senders,
        string email,
        CancellationToken ct = default
    )
    {
        IEmailSender sender = await senders.Get(email, ct);
        await sender.Remove(senders, ct);
        return sender.Print();
    }
}
