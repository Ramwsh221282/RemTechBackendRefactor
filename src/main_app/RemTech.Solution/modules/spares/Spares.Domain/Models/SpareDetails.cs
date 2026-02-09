namespace Spares.Domain.Models;

/// <summary>
/// Детали запчасти.
/// </summary>
/// <param name="Text">Текстовая информация о запчасти.</param>
/// <param name="Price">Цена запчасти.</param>
/// <param name="Address">Адрес запчасти.</param>
/// <param name="Photos">Коллекция фотографий запчасти.</param>
public sealed record SpareDetails(
	SpareTextInfo Text,
	SparePrice Price,
	SpareAddress Address,
	SparePhotoCollection Photos
);
