using Cleaners.Module.Domain;
using RemTech.Core.Shared.Cqrs;

namespace Cleaners.Module.Services.Features.FinishJob;

internal sealed class FinishJobHandler(Serilog.ILogger logger, ICleaners cleaners)
    : ICommandHandler<FinishJobCommand>
{
    public async Task Handle(FinishJobCommand command, CancellationToken ct = default)
    {
        ICleaner cleaner = await cleaners.Single(ct);
        ICleaner finished = cleaner.StopWork(command.Seconds);
    }
}
