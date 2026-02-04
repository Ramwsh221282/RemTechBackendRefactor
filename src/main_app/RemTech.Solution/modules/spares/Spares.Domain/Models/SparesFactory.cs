using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Spares.Domain.Oems;
using Spares.Domain.Types;

namespace Spares.Domain.Models;

/// <summary>
/// Фабрика для создания экземпляров запчастей.
/// </summary>
public static class SparesFactory
{
	/// <summary>
	/// Создаёт OEM запчасти из строки, где строка - какой-то артикул.
	/// </summary>
	public static Result<SpareOem> CreateSpareOem(string value)
	{
		Result<SpareOem> oem = SpareOem.Create(value);
		if (oem.IsFailure)
		{
			return oem.Error;
		}

		return oem.Value;
	}

	/// <summary>
	/// Создаёт тип запчасти из строки, где строка - какой-то тип запчасти.
	/// </summary>
	public static Result<SpareType> CreateSpareType(string value)
	{
		Result<SpareType> type = SpareType.Create(value);
		if (type.IsFailure)
		{
			return type.Error;
		}

		return type.Value;
	}

	/// <summary>
	/// Создаёт экземпляр запчасти на основе переданных параметров.
	/// </summary>
	/// <param name="containedItemId">Идентификатор вложенного элемента.</param>
	/// <param name="source">Источник информации о запчасти.</param>
	/// <param name="oem">OEM-номер запчасти.</param>
	/// <param name="title">Описание запчасти.</param>
	/// <param name="price">Цена запчасти.</param>
	/// <param name="isNds">Признак наличия НДС.</param>
	/// <param name="type">Тип запчасти.</param>
	/// <param name="address">Адрес хранения запчасти.</param>
	/// <param name="photoPaths">Коллекция фото запчасти.</param>
	/// <returns>Результат создания запчасти.</returns>
	public static Result<Spare> Create(
		SpareOem oem,
		SpareType type,
		Guid containedItemId,
		string source,
		string title,
		long price,
		bool isNds,
		string address,
		IEnumerable<string> photoPaths
	)
	{
		Result<ContainedItemId> itemId = ContainedItemId.Create(containedItemId);
		if (itemId.IsFailure)
		{
			return itemId.Error;
		}

		Result<SpareTextInfo> textResult = SpareTextInfo.Create(title);
		if (textResult.IsFailure)
		{
			return textResult.Error;
		}

		Result<SparePrice> priceResult = SparePrice.Create(price, isNds);
		if (priceResult.IsFailure)
		{
			return priceResult.Error;
		}

		Result<SparePhotoCollection> photosResult = SparePhotoCollection.Create(photoPaths);
		if (photosResult.IsFailure)
		{
			return photosResult.Error;
		}

		Result<SpareSource> sourceResult = SpareSource.Create(source);
		if (sourceResult.IsFailure)
		{
			return sourceResult.Error;
		}

		Result<SpareAddress> addressResult = SpareAddress.Create(address);
		if (addressResult.IsFailure)
		{
			return addressResult.Error;
		}

		SpareDetails details = new(
			Text: textResult.Value,
			Price: priceResult.Value,
			Address: addressResult.Value,
			Photos: photosResult.Value
		);

		return new Spare(
			Id: itemId.Value,
			OemId: oem.Id,
			TypeId: type.Id,
			Details: details,
			Source: sourceResult.Value
		);
	}
}
