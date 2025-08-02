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
        [FromQuery] string name,
        [FromServices] IEmailSendersSource senders,
        CancellationToken ct
    )
    {
        EmailSenderOutput output = await Process(senders, name, ct);
        RemoveMailingSenderResponse response = new(output.Name, output.Email);
        return Results.Ok(response);
    }

    private static async Task<EmailSenderOutput> Process(
        IEmailSendersSource senders,
        string name,
        CancellationToken ct = default
    )
    {
        IEmailSender sender = await senders.Get(name, ct);
        await sender.Remove(senders, ct);
        return sender.Print();
    }
}
