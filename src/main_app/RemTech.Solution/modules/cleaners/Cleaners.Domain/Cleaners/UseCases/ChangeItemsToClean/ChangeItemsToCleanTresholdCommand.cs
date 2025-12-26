using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Domain.Cleaners.UseCases.ChangeItemsToClean;

public sealed record ChangeItemsToCleanTresholdCommand(Guid Id, int Threshold) : ICommand;
