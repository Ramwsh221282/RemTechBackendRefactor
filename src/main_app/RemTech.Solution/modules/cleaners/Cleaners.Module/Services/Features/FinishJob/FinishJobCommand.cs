using Shared.Infrastructure.Module.Cqrs;

namespace Cleaners.Module.Services.Features.FinishJob;

internal sealed record FinishJobCommand(long Seconds) : ICommand;
