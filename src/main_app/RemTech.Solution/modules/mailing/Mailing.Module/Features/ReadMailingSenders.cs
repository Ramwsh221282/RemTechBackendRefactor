using Mailing.Module.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mailing.Module.Features;

public static class ReadMailingSenders
{
    public sealed record ReadMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] IEmailSendersSource senders,
        CancellationToken ct
    )
    {
        IEnumerable<EmailSenderOutput> entries = await Process(senders, ct);
        IEnumerable<ReadMailingSenderResponse> responses = entries.Select(
            e => new ReadMailingSenderResponse(e.Name, e.Email)
        );
        return Results.Ok(responses);
    }

    private static async Task<IEnumerable<EmailSenderOutput>> Process(
        IEmailSendersSource senders,
        CancellationToken ct
    )
    {
        IEnumerable<IEmailSender> entries = await senders.ReadAll(ct);
        IEnumerable<EmailSenderOutput> outputs = entries.Select(e => e.Print());
        return outputs;
    }
}
