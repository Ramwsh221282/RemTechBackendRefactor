namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

/// <summary>
/// Элемент последних добавленных элементов на главную страницу.
/// </summary>
/// <param name="Spare">Данные запчасти.</param>
/// <param name="Vehicle">Данные техники.</param>
public sealed record MainPageLastAddedItem(SpareData? Spare, VehicleData? Vehicle);
