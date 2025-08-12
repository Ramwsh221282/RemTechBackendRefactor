using Shared.Infrastructure.Module.Cqrs;

namespace Categories.Module.Features.QueryCategoriesAmount;

internal sealed record QueryCategoriesAmountCommand(string? Text = null) : ICommand;
