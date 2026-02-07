using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Spares.Domain.Oems;
using Spares.Domain.Types;

namespace Spares.Domain.Models;

/// <summary>
/// Запчасть.
/// </summary>
/// <param name="Id">Идентификатор запчасти.</param>
/// <param name="Details">Детали запчасти.</param>
/// <param name="Source">Источник запчасти.</param>
/// 
public sealed record Spare(
    ContainedItemId Id, 
    SpareOem Oem, 
    SpareType Type, 
    SpareDetails Details, 
    SpareSource Source,
    SpareAddressId? AddressId = null
    );

public static class SpareImplementation
{
    extension(Spare spare)
    {
        public bool HasAddress()
        {
            return spare.AddressId is not null;
        }

        public Spare WithAddress(SpareAddressId id, SpareAddress address)
        {
            return spare with
            {
                AddressId = id,
                Details = spare.Details with { Address = address }
            };
        }

        public Result<Spare> WithAddress(Guid id, string address)
        {
            Result<SpareAddressId> idResult = SpareAddressId.Create(id);
            if (idResult.IsFailure)
            {
                return idResult.Error;
            }
            
            Result<SpareAddress> addressResult = SpareAddress.Create(address);
            if (addressResult.IsFailure)
            {
                return addressResult.Error;
            }
            
            return spare.WithAddress(idResult.Value, addressResult.Value);
        }
    }
}


