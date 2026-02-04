using System.Text.Json;
using Spares.Domain.Models;

namespace Spares.Infrastructure.Extensions;

/// <summary>
/// Расширения для инфраструктуры запчастей.
/// </summary>
public static class SparesInfrastructureExtensions
{
	extension(Spare spare)
	{
		public object ExtractForParameters()
		{
			return new
			{
				id = spare.Id.Value,
				contained_item_id = spare.Id.Value,
				url = spare.Source.Url,
				content = JsonSerializer.Serialize(
					new
					{
						oem_id = spare.Oem.Id.Value,
						title = spare.Details.Text.Value,
						price = spare.Details.Price.Value,
						is_nds = spare.Details.Price.IsNds,
						type_id = spare.Type.Id.Value,
						address = spare.Details.Address.Value,
						photos = spare.Details.Photos.Value.Select(p => p.Value).ToArray(),
					}
				),
			};
		}
	}
}
