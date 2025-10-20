using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Telemetry.Domain.Models;
using Telemetry.Domain.Models.ValueObjects;
using Telemetry.Domain.Ports.Storage;
using Telemetry.Domain.UseCases.AddAction.Extensions;
using Telemetry.Domain.UseCases.AddAction.Input;
using Telemetry.Domain.UseCases.AddAction.Output;

namespace Telemetry.Domain.UseCases.AddAction.Handlers;

/// <summary>
/// Обработчик команды добавления действия
/// </summary>
public sealed class AddActionHandler : ICommandHandler<AddActionCommand, Status<ActionRecordOutput>>
{
    private readonly IActionRecordsStorage _storage;

    public AddActionHandler(IActionRecordsStorage storage) => _storage = storage;

    public async Task<Status<ActionRecordOutput>> Handle(
        AddActionCommand command,
        CancellationToken ct = default
    )
    {
        ActionInvokerId id = ActionInvokerId.Create(command.InvokerId);
        ActionName name = ActionName.Create(command.Name);
        ActionStatus status = ActionStatus.Create(command.Status);
        ActionDate date = ActionDate.Create(command.OccuredAt);
        IReadOnlyList<ActionComment> comments =
        [
            .. ActionComment.Create(command.Comments).Value.Select(v => v),
        ];

        ActionRecord record = new ActionRecord
        {
            InvokerId = id,
            Name = name,
            Status = status,
            Comments = comments,
            OccuredAt = date,
            Id = new ActionId(),
        };

        await _storage.Add(record, ct);
        return record.ToOutput();
    }
}
