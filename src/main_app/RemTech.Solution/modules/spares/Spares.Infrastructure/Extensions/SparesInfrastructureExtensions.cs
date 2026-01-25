using System.Text.Json;
using Spares.Domain.Models;

namespace Spares.Infrastructure.Extensions;

public static class SparesInfrastructureExtensions
{
    extension(Spare spare)
    {
        public object ExtractForParameters() =>
            new
            {
                id = spare.Id.Value,
                contained_item_id = spare.Id.Value,
                url = spare.Source.Url,
                content = JsonSerializer.Serialize(
                    new
                    {
                        oem = spare.Details.Oem.Value,
                        title = spare.Details.Text.Value,
                        price = spare.Details.Price.Value,
                        is_nds = spare.Details.Price.IsNds,
                        type = spare.Details.Type.Value,
                        address = spare.Details.Address.Value,
                        photos = spare.Details.Photos.Value.Select(p => p.Value).ToArray(),
                    }
                ),
            };
    }
}
