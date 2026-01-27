namespace Spares.Domain.Models;

// TODO: Вынести Spare Type в отдельную сущность (в дальнейшем это будет справочник всех типов запчастей, чтобы избежать дублирования).
// TODO: Вынести Spare Oem в отдельную сущность (в дальнейшем это будет справочник всех oem, чтобы избежать дублирования).
// TODO: Вынести Spare Oem не создавать, если он не содержит OEM (например Ponsse 321321321), в OEM должен быть только артикул.
// TODO: Spare Oem не создавать, если он содержит только нули (0 или 000) в артикуле.
// TODO: парсер должен парсить фотки.
/// <summary>
/// Детали запчасти.
/// </summary>
/// <param name="Oem">OEM запчасти (артикул).</param>
/// <param name="Text">Текстовая информация о запчасти.</param>
/// <param name="Price">Цена запчасти.</param>
/// <param name="Type">Тип запчасти.</param>
/// <param name="Address">Адрес запчасти.</param>
/// <param name="Photos">Коллекция фотографий запчасти.</param>
public sealed record SpareDetails(
	SpareOem Oem,
	SpareTextInfo Text,
	SparePrice Price,
	SpareType Type,
	SpareAddress Address,
	SparePhotoCollection Photos
);
