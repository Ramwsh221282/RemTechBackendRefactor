using Mailing.Module.Cache;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using StackExchange.Redis;

namespace Mailing.Module.Features;

public static class ReadMailingSenders
{
    public sealed record ReadMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] ConnectionMultiplexer multiplexer,
        CancellationToken ct
    )
    {
        MailingSendersCache cache = new(multiplexer);
        CachedMailingSender[] senders = await cache.GetAll();
        IEnumerable<ReadMailingSenderResponse> responses = senders.Select(
            e => new ReadMailingSenderResponse(e.Name, e.Email)
        );
        return Results.Ok(responses);
    }
}
