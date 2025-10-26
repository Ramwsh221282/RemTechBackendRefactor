using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.ChangeItemsToClean;

public sealed record ChangeItemsToCleanTreshold(Guid Id, int Threshold) : ICommand;
