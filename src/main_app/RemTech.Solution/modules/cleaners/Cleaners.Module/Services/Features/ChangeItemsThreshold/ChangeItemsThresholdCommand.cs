using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.ChangeItemsThreshold;

internal sealed record ChangeItemsThresholdCommand(int Threshold) : ICommand;
