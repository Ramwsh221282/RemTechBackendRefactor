using FluentValidation;
using FluentValidation.Results;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Cqrs;
using RemTech.UseCases.Shared.Logging;
using RemTech.UseCases.Shared.Validations;
using Serilog;
using Telemetry.Domain.TelemetryContext;
using Telemetry.Domain.TelemetryContext.Contracts;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.UseCases.SaveActionInfo;

/// <summary>
/// Обработчик команды добавления действия
/// </summary>
public sealed class SaveActionInfoIbCommandHandler
    : ICommandHandler<SaveActionInfoIbCommand, TelemetryRecord>
{
    private readonly ITelemetryRecordsRepository _recordsRepository;
    private readonly IValidator<SaveActionInfoIbCommand> _validator;
    private readonly ILogger _logger;

    public SaveActionInfoIbCommandHandler(
        ITelemetryRecordsRepository recordsRepository,
        IValidator<SaveActionInfoIbCommand> validator,
        ILogger logger
    )
    {
        _recordsRepository = recordsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<TelemetryRecord>> Handle(
        SaveActionInfoIbCommand ibCommand,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = await _validator.ValidateAsync(ibCommand, ct);
        if (validation.NotValid())
            return _logger.LoggedError(validation.Failure<TelemetryRecord>());

        TelemetryActionName name = TelemetryActionName.Create(ibCommand.Name);
        TelemetryActionStatus status = TelemetryActionStatus.Create(ibCommand.Status);
        IEnumerable<TelemetryActionComment> comments =
        [
            .. ibCommand.Comments.Select(TelemetryActionComment.Create),
        ];

        TelemetryInvokerId invokerId = TelemetryInvokerId.Create(ibCommand.InvokerId);
        TelemetryRecordDate occured = TelemetryRecordDate.Create(ibCommand.OccuredAt);
        TelemetryRecord record = new TelemetryRecord(invokerId, name, comments, status, occured);

        await record.Save(_recordsRepository, ct);
        _logger.Information(
            "Saved telemetry record: {Id} Occured: {Occured} Action: {Name} By: {By}",
            record.RecordId.Value,
            ibCommand.OccuredAt,
            ibCommand.Name,
            ibCommand.InvokerId
        );
        return record;
    }
}
