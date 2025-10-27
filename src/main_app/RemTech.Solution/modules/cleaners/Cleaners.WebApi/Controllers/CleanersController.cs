using Cleaners.Domain.Cleaners.UseCases.ChangeItemsToClean;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.Disable;
using Cleaners.Domain.Cleaners.UseCases.StartWait;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Cleaners.Domain.Cleaners.UseCases.UpdateSchedule;
using Cleaners.WebApi.Extensions;
using Cleaners.WebApi.Responses;
using Identity.Contracts;
using Microsoft.AspNetCore.Mvc;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.WebApi;

namespace Cleaners.WebApi.Controllers;

[Route("api/cleaners")]
[ApiController]
public sealed class CleanersController : ControllerBase
{
    [TypeFilter<RoleAccessFilter>(Arguments = [DefaultRoleNames.Admin, DefaultRoleNames.Root])]
    [HttpPatch("{id:guid}/threshold")]
    public async Task<IResult> ChangeCleanerThreshold(
        [FromRoute] Guid id,
        [FromQuery(Name = "threshold")] int threshold,
        [FromServices]
            ICommandHandler<
            ChangeItemsToCleanTresholdCommand,
            Status<Domain.Cleaners.Aggregate.Cleaner>
        > handler,
        CancellationToken ct
    )
    {
        var command = new ChangeItemsToCleanTresholdCommand(id, threshold);
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? new HttpEnvelope<CleanerDto>(result)
            : new HttpEnvelope<CleanerDto>(result.ToDto());
    }

    [TypeFilter<RoleAccessFilter>(Arguments = [DefaultRoleNames.Admin, DefaultRoleNames.Root])]
    [HttpPost]
    public async Task<IResult> CreateCleaner(
        ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>> handler,
        CancellationToken ct
    )
    {
        var command = new CreateCleanerCommand();
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? new HttpEnvelope<CleanerDto>(result)
            : new HttpEnvelope<CleanerDto>(result.ToDto());
    }

    [TypeFilter<RoleAccessFilter>(Arguments = [DefaultRoleNames.Admin, DefaultRoleNames.Root])]
    [HttpPatch("{id:guid}/disabled")]
    public async Task<IResult> DisableCleaner(
        [FromRoute] Guid id,
        ICommandHandler<DisableCommand, Status<Domain.Cleaners.Aggregate.Cleaner>> handler,
        CancellationToken ct
    )
    {
        var command = new DisableCommand(id);
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? new HttpEnvelope<CleanerDto>(result)
            : new HttpEnvelope<CleanerDto>(result.ToDto());
    }

    [TypeFilter<RoleAccessFilter>(Arguments = [DefaultRoleNames.Admin, DefaultRoleNames.Root])]
    [HttpPatch("{id:guid}/enabled")]
    public async Task<IResult> EnableCleaner(
        [FromRoute] Guid id,
        ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>> handler,
        CancellationToken ct
    )
    {
        var command = new StartWorkCommand(id);
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? new HttpEnvelope<CleanerDto>(result)
            : new HttpEnvelope<CleanerDto>(result.ToDto());
    }

    [TypeFilter<RoleAccessFilter>(Arguments = [DefaultRoleNames.Admin, DefaultRoleNames.Root])]
    [HttpPatch("{id:guid}/waiting")]
    public async Task<IResult> MakeCleanerWaiting(
        [FromRoute] Guid id,
        ICommandHandler<StartWaitCommand, Status<Domain.Cleaners.Aggregate.Cleaner>> handler,
        CancellationToken ct
    )
    {
        var command = new StartWaitCommand(id);
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? new HttpEnvelope<CleanerDto>(result)
            : new HttpEnvelope<CleanerDto>(result.ToDto());
    }

    [HttpPut("{id:guid}/schedule")]
    public async Task<IResult> UpdateCleanerSchedule(
        [FromRoute] Guid id,
        [FromRoute(Name = "waitDays")] int waitDays,
        ICommandHandler<UpdateScheduleCommand, Status<Domain.Cleaners.Aggregate.Cleaner>> handler,
        CancellationToken ct
    )
    {
        var command = new UpdateScheduleCommand(id, waitDays);
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? new HttpEnvelope<CleanerDto>(result)
            : new HttpEnvelope<CleanerDto>(result.ToDto());
    }
}
