using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Фабрика для создания экземпляров запчастей.
/// </summary>
public static class SparesFactory
{
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
		Guid containedItemId,
		string source,
		string oem,
		string title,
		long price,
		bool isNds,
		string type,
		string address,
		IEnumerable<string> photoPaths
	)
	{
		Result<ContainedItemId> itemId = ContainedItemId.Create(containedItemId);
		if (itemId.IsFailure)
			return itemId.Error;

		Result<SpareOem> oemResult = SpareOem.Create(oem);
		if (oemResult.IsFailure)
			return oemResult.Error;

		Result<SpareTextInfo> textResult = SpareTextInfo.Create(title);
		if (textResult.IsFailure)
			return textResult.Error;

		Result<SparePrice> priceResult = SparePrice.Create(price, isNds);
		if (priceResult.IsFailure)
			return priceResult.Error;

		Result<SparePhotoCollection> photosResult = SparePhotoCollection.Create(photoPaths);
		if (photosResult.IsFailure)
			return photosResult.Error;

		Result<SpareSource> sourceResult = SpareSource.Create(source);
		if (sourceResult.IsFailure)
			return sourceResult.Error;

		Result<SpareType> typeResult = SpareType.Create(type);
		if (typeResult.IsFailure)
			return typeResult.Error;

		Result<SpareAddress> addressResult = SpareAddress.Create(address);
		return addressResult.IsFailure
			? addressResult.Error
			: new Spare(
				Id: itemId.Value,
				Details: new SpareDetails(
					Oem: oemResult.Value,
					Text: textResult.Value,
					Price: priceResult.Value,
					Type: typeResult.Value,
					Address: addressResult.Value,
					Photos: photosResult.Value
				),
				Source: sourceResult.Value
			);
	}
}
