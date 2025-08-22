using Mailing.Module.Cache;
using Mailing.Module.Contracts;
using Mailing.Module.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Serilog;
using StackExchange.Redis;

namespace Mailing.Module.Features;

public static class CreateMailingSender
{
    public sealed record CreateMailingSenderRequest(string Email, string Password);

    public sealed record CreateMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapPost(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromBody] CreateMailingSenderRequest request,
        [FromServices] IEmailSendersSource senders,
        [FromServices] ILogger logger,
        [FromServices] ConnectionMultiplexer multiplexer,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new InvalidOperationException("Email is required.");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Password is required.");
        MailingSendersCache cache = new(multiplexer);
        IEmailSender sender = await Create(senders, request, cache, ct);
        EmailSenderOutput output = sender.Print();
        CreateMailingSenderResponse response = new(output.Name, output.Email);
        logger.Information("Created mailing sender: {Name} - {Email}", output.Name, output.Email);
        return Results.Ok(response);
    }

    private static async Task<IEmailSender> Create(
        IEmailSendersSource senders,
        CreateMailingSenderRequest request,
        MailingSendersCache cache,
        CancellationToken ct = default
    )
    {
        IEmailSender sender = new EmailSender(request.Email, request.Password);
        await sender.Save(senders, ct);
        await cache.Add(sender);
        return sender;
    }
}
