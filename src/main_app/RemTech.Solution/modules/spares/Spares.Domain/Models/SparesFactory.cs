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
        if (idResult.IsFailure) return idResult.Error;
        
        Result<ContainedItemId> itemId = ContainedItemId.Create(containedItemId);
        if (itemId.IsFailure) return itemId.Error;
        
        Result<SpareOem> oemResult = SpareOem.Create(oem);
        if (oemResult.IsFailure) return oemResult.Error;
        
        Result<SpareTextInfo> textResult = SpareTextInfo.Create(title);
        if (textResult.IsFailure) return textResult.Error;
        
        Result<SparePrice> priceResult = SparePrice.Create(price, isNds);
        if (priceResult.IsFailure) return priceResult.Error;
        
        Result<SparePhotoCollection> photosResult = SparePhotoCollection.Create(photoPaths);
        if (photosResult.IsFailure) return photosResult.Error;
        
        Result<SpareSource> sourceResult = SpareSource.Create(source);
        if (sourceResult.IsFailure) return sourceResult.Error;
        
        Result<SpareType> typeResult = SpareType.Create(type);
        if (typeResult.IsFailure) return typeResult.Error;
        
        Result<SpareAddress> addressResult = SpareAddress.Create(address);
        if (addressResult.IsFailure) return addressResult.Error;
        
        return new Spare(
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
        );
    }
}