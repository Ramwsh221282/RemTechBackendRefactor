using Mailing.Module.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mailing.Module.Features;

public static class PingMailingSender
{
    public sealed record PingMailingSenderRequest(string Email, string To);

    public sealed record PingMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapPost("ping", Handle);

    private static async Task<IResult> Handle(
        [FromServices] IEmailSendersSource senders,
        [FromBody] PingMailingSenderRequest request,
        CancellationToken ct
    )
    {
        EmailSenderOutput output = await Process(senders, request, ct);
        return Results.Ok(new PingMailingSenderResponse(output.Name, output.Email));
    }

    private static async Task<EmailSenderOutput> Process(
        IEmailSendersSource senders,
        PingMailingSenderRequest request,
        CancellationToken ct
    )
    {
        IEmailSender sender = await senders.Get(request.Email, ct);
        await sender
            .FormEmailMessage()
            .Send(request.To, "RemTech Test Email Sending.", "Do not answer for this message.", ct);
        return sender.Print();
    }
}
