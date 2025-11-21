using System.Text.Json;
using RemTech.Functional.Extensions;

namespace Tickets.EventListeners.Routers.RequireActivationTicket;

public sealed class JsonRequiredActivationTicketEmail : RequireActivationTicketEmail
{
    private readonly string _json;
    
    public Result<string> Email()
    {
        using JsonDocument document = JsonDocument.Parse(_json);
        JsonElement root = document.RootElement;
        if (!root.TryGetProperty("required_email", out JsonElement emailElement))
            return Error.Conflict("Extra information doesn't contain required_email property.");
        
        string? requiredEmail = emailElement.GetString();
        if (string.IsNullOrWhiteSpace(requiredEmail))
            return Error.Conflict("Extra information doesn't contain required_email property.");
        
        return requiredEmail;
    }

    public JsonRequiredActivationTicketEmail(string? json)
    {
        _json = json ?? string.Empty;
    }
}