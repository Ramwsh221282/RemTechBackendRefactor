using Shared.Infrastructure.Module.Cqrs;

namespace Categories.Module.Features.GetCategory;

internal sealed record GetCategoryCommand(string Name) : ICommand;
