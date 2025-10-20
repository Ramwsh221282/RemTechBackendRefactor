using RemTech.Core.Shared.Cqrs;

namespace Categories.Module.Features.QueryCategories;

internal sealed record QueryCategoriesCommand(int Page, string? Text = null) : ICommand;
