using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RemTech.BuildingBlocks.DependencyInjection;
using RemTech.Functional.Extensions;
using Tickets.Core;
using Tickets.Core.Contracts;

namespace Tickets.EventListeners;

public abstract record TicketRequiredMessage
{
    public sealed record ValidRequiredMessage(Guid CreatorId, Guid TicketId, string Type, string? Json) : TicketRequiredMessage
    {
        public override async Task<Result<Ticket>> Register(IServiceProvider sp)
        {
            RegisterTicketArgs args = new(CreatorId, TicketId, Type, Json, CancellationToken.None);
            await using AsyncServiceScope scope = sp.CreateAsyncScope();
            RegisterTicket useCase = scope.Resolve<RegisterTicket>();
            return await useCase(args);
        }
    }

    public sealed record InvalidRequiredMessage : TicketRequiredMessage
    {
        public override Task<Result<Ticket>> Register(IServiceProvider sp)
        {
            return Task.FromResult<Result<Ticket>>(Error.Conflict("Ticket body was invalid format."));
        }
    }

    public static TicketRequiredMessage FromRabbitMqJson(JsonMessageFromRabbitMq message)
    {
        bool hasCreatorId = message.TryGetProperty("creator_id", out JsonElement creatorIdElement);
        bool hasTicketId = message.TryGetProperty("ticket_id", out JsonElement ticketIdElement);
        bool hasType = message.TryGetProperty("type", out JsonElement typeElement);
        bool hasExtra = message.TryGetProperty("extra", out JsonElement extraElement);

        string? json = null;
        if (hasExtra)
        {
            object? extra = extraElement.Deserialize<object>();
            json = extra == null ? null : JsonSerializer.Serialize(extra);
        }
        
        return (hasCreatorId, hasTicketId, hasType) switch
        {
            (true, true, true) => 
                new ValidRequiredMessage(creatorIdElement.GetGuid(), ticketIdElement.GetGuid(), typeElement.GetString()!, json),
            _ => new InvalidRequiredMessage()
        };
    }
    
    public abstract Task<Result<Ticket>> Register(IServiceProvider sp);
}