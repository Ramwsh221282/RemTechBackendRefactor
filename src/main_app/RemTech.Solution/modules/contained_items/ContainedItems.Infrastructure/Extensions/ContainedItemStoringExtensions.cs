using ContainedItems.Domain.Models;

namespace ContainedItems.Infrastructure.Extensions;

public static class ContainedItemStoringExtensions
{
    extension(IEnumerable<ContainedItem> items)
    {
        internal object[] ExtractForParameters()
        {
            return items.Select(i => i.ExtractForParameter()).ToArray();
        }
    }
    
    extension(ContainedItem item)
    {
        internal object ExtractForParameter() => new
        {
            item_type = item.ItemType.Value,
            id = item.Id.Value,
            service_item_id = item.ServiceItemId.Value,
            creator_id = item.CreatorInfo.CreatorId,
            creator_type = item.CreatorInfo.Type,
            creator_domain = item.CreatorInfo.Domain,
            content = item.Info.Content,
            created_at = item.Info.CreatedAt,
            deleted_at = item.Info.DeletedAt.HasValue ? item.Info.DeletedAt.Value : (DateTime?)null,
            status = item.Status.Value
        };   
    }
}