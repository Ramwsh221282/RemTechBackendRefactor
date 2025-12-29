using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public static class SparesFactory
{
    public static Result<Spare> Create(
        string spareId,
        Guid containedItemId,
        string source,
        string oem,
        string title,
        long price,
        bool isNds,
        string type,
        string address,
        IEnumerable<string> photoPaths)
    {
        Result<SpareId> idResult = SpareId.Create(spareId);
        Result<ContainedItemId> itemId = idResult.Map(() => ContainedItemId.Create(containedItemId));
        Result<SpareOem> oemResult = itemId.Map(() => SpareOem.Create(oem));
        Result<SpareTextInfo> textResult = oemResult.Map(() => SpareTextInfo.Create(title));
        Result<SparePrice> priceResult = textResult.Map(() => SparePrice.Create(price, isNds));
        Result<SparePhotoCollection> photosResult = priceResult.Map(() => SparePhotoCollection.Create(photoPaths));
        Result<SpareSource> sourceResult = photosResult.Map(() => SpareSource.Create(source));
        Result<SpareType> typeResult = sourceResult.Map(() => SpareType.Create(type));
        Result<SpareAddress> addressResult = typeResult.Map(() => SpareAddress.Create(address));
        return addressResult.Map(() => new Spare(
            Source: sourceResult.Value,
            Id: idResult.Value,
            ContainedItemId: itemId.Value,
            Details: new SpareDetails(
                Oem: oemResult.Value, 
                Text: textResult.Value, 
                Price: priceResult.Value, 
                Type: typeResult.Value, 
                Address: addressResult.Value, 
                Photos: photosResult.Value)
        ));
    }
}