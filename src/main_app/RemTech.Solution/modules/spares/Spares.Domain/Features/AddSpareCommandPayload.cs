namespace Spares.Domain.Features;

/// <summary>
///  Полезная нагрузка команды добавления запчасти.
/// </summary>
/// <param name="ContainedItemId">Идентификатор содержащего элемента.</param>
/// <param name="Source">Источник запчасти.</param>
/// <param name="Oem">OEM номер запчасти.</param>
/// <param name="Title">Название запчасти.</param>
/// <param name="Price">Цена запчасти.</param>
/// <param name="IsNds">Наличие НДС.</param>
/// <param name="Address">Адрес запчасти.</param>
/// <param name="Type">Тип запчасти.</param>
/// <param name="PhotoPaths">Пути к фотографиям запчасти.</param>
public sealed record AddSpareCommandPayload(
	Guid ContainedItemId,
	string Source,
	string Oem,
	string Title,
	long Price,
	bool IsNds,
	string Address,
	string Type,
	IEnumerable<string> PhotoPaths
);
