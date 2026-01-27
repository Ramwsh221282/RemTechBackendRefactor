using RemTech.SharedKernel.Core.Handlers;

namespace ContainedItems.Domain.Features.AddContainedItems;

/// <summary>
/// Команда для добавления содержащихся элементов.
/// </summary>
/// <param name="Items">Коллекция содержащихся элементов для добавления.</param>
public sealed record AddContainedItemsCommand(IEnumerable<AddContainedItemsBody> Items) : ICommand;
