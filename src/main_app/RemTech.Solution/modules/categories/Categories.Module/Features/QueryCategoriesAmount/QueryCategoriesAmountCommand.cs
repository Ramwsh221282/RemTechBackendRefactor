using RemTech.Core.Shared.Cqrs;

namespace Categories.Module.Features.QueryCategoriesAmount;

internal sealed record QueryCategoriesAmountCommand(string? Text = null) : ICommand;
