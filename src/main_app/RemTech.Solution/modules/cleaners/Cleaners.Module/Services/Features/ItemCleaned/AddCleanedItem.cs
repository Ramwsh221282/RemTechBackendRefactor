using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.ItemCleaned;

internal sealed record AddCleanedItem(string Id) : ICommand;
