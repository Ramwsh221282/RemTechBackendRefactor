namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

/// <summary>
/// Данные запчасти.
/// </summary>
/// <param name="Id">Идентификатор запчасти.</param>
/// <param name="Title">Название запчасти.</param>
public sealed record SpareData(Guid Id, string Title);
