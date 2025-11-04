using System.Text.Json;
using Mailing.Domain.Postmans;

namespace Mailing.Adapters.Cache.Postmans;

internal sealed class JsonPostman(string? json = null, IPostman? postman = null)
{
    public string Serialize()
    {
        if (postman is null)
            throw new ApplicationException("Cannot serialize postman object. Postman object is null.");

        IPostmanData data = postman.Data;

        JsonPostmanStructure structure = new()
        {
            Id = data.Id,
            Email = data.Email,
            SmtpPassword = data.SmtpPassword,
        };

        return JsonSerializer.Serialize(structure);
    }

    public IPostman Deserialize()
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ApplicationException("Cannot deserialize postman object. Postman json is null.");

        JsonPostmanStructure? structure = JsonSerializer.Deserialize<JsonPostmanStructure>(json);
        return structure is null
            ? throw new ApplicationException("Cannot parse postman structure object from cache")
            : structure.Cached();
    }
}