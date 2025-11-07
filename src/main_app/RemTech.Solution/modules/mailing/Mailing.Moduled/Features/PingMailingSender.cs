using Mailing.Moduled.Models;
using Mailing.Moduled.Cache;
using Mailing.Moduled.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;

namespace Mailing.Moduled.Features;

internal sealed class MailingSenderNotFoundException : Exception
{
    public MailingSenderNotFoundException(string email)
        : base($"Отправитель {email} не найден.")
    {
    }
}

internal sealed class MailPingMessageRecepientEmptyException : Exception
{
    public MailPingMessageRecepientEmptyException()
        : base("Не указан получатель отправки почты.")
    {
    }
}

internal sealed class MailingPingMessageSenderEmptyException : Exception
{
    public MailingPingMessageSenderEmptyException()
        : base("Не указан сервис отправки почты.")
    {
    }
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
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new MailingPingMessageSenderEmptyException();
            if (string.IsNullOrWhiteSpace(request.To))
                throw new MailPingMessageRecepientEmptyException();
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
        IEmailSender sender = await GetRequiredSender(request, multiplexer);
        await sender
            .FormEmailMessage()
            .Send(request.To, "RemTech Test Email Sending.", "Do not answer for this message.", ct);
        return sender.Print();
    }

    private static async Task<IEmailSender> GetRequiredSender(
        PingMailingSenderRequest request,
        ConnectionMultiplexer multiplexer
    )
    {
        MailingSendersCache cache = new(multiplexer);
        CachedMailingSender[] senders = await cache.GetAll();
        foreach (CachedMailingSender entry in senders)
        {
            if (entry.Email == entry.Email)
                return entry.AsSender();
        }

        throw new MailingSenderNotFoundException(request.Email);
    }
}