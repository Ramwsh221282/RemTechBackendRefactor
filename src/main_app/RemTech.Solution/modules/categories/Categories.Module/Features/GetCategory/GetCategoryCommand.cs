using RemTech.Core.Shared.Cqrs;

namespace Categories.Module.Features.GetCategory;

internal sealed record GetCategoryCommand(string Name) : ICommand;
