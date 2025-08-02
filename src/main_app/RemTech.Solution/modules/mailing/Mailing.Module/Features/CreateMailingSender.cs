using Mailing.Module.Contracts;
using Mailing.Module.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Mailing.Module.Features;

public static class CreateMailingSender
{
    public sealed record CreateMailingSenderRequest(string Name, string Email, string Password);

    public sealed record CreateMailingSenderResponse(string Name, string Email);

    public static void Map(RouteGroupBuilder builder) => builder.MapPost(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromQuery] string name,
        [FromQuery] string email,
        [FromHeader] string password,
        [FromServices] IEmailSendersSource senders,
        CancellationToken ct
    )
    {
        CreateMailingSenderRequest request = new(name, email, password);
        IEmailSender sender = await Create(senders, request, ct);
        EmailSenderOutput output = sender.Print();
        CreateMailingSenderResponse response = new(output.Name, output.Email);
        return Results.Ok(response);
    }

    private static async Task<IEmailSender> Create(
        IEmailSendersSource senders,
        CreateMailingSenderRequest request,
        CancellationToken ct = default
    )
    {
        IEmailSender sender = new EmailSender(request.Name, request.Email, request.Password);
        await sender.Save(senders, ct);
        return sender;
    }
}
