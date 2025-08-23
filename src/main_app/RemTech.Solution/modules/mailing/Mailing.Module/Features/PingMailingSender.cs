using Mailing.Module.Cache;
using Mailing.Module.Contracts;
using Mailing.Module.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;

namespace Mailing.Module.Features;

internal sealed class MailingSenderNotFoundException : Exception
{
    public MailingSenderNotFoundException(string email)
        : base($"Отправитель {email} не найден.") { }
}

public static class PingMailingSender
{
    public sealed record PingMailingSenderRequest(string Email, string To);

    public sealed record PingMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapPost("ping", Handle);

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromBody] PingMailingSenderRequest request,
        CancellationToken ct
    )
    {
        try
        {
            EmailSenderOutput output = await Process(multiplexer, request, ct);
            return Results.Ok(new PingMailingSenderResponse(output.Name, output.Email));
        }
        catch (MailingSenderNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<EmailSenderOutput> Process(
        ConnectionMultiplexer multiplexer,
        PingMailingSenderRequest request,
        CancellationToken ct
    )
    {
        MailingSendersCache cache = new(multiplexer);
        CachedMailingSender[] senders = await cache.GetAll();
        foreach (var senderEntry in senders)
        {
            if (senderEntry.Email != request.Email)
                continue;
            IEmailSender sender = new EmailSender(senderEntry.Email, senderEntry.Key);
            await sender
                .FormEmailMessage()
                .Send(
                    request.To,
                    "RemTech Test Email Sending.",
                    "Do not answer for this message.",
                    ct
                );
            return sender.Print();
        }

        throw new MailingSenderNotFoundException(request.Email);
    }
}
