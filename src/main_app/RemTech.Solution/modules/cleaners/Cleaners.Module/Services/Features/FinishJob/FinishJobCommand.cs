using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.FinishJob;

internal sealed record FinishJobCommand(long Seconds) : ICommand;
