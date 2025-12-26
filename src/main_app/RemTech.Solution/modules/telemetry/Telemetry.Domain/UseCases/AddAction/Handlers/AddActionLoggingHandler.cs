using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Serilog;
using Telemetry.Domain.UseCases.AddAction.Input;
using Telemetry.Domain.UseCases.AddAction.Output;

namespace Telemetry.Domain.UseCases.AddAction.Handlers;

public sealed class AddActionLoggingHandler
    : ICommandHandler<AddActionCommand, Status<ActionRecordOutput>>
{
    private const string Context = nameof(AddActionCommand);
    private readonly ICommandHandler<AddActionCommand, Status<ActionRecordOutput>> _handler;
    private readonly ILogger _logger;

    public AddActionLoggingHandler(
        ILogger logger,
        ICommandHandler<AddActionCommand, Status<ActionRecordOutput>> handler
    )
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Status<ActionRecordOutput>> Handle(
        AddActionCommand command,
        CancellationToken ct = default
    )
    {
        Status<ActionRecordOutput> status = await _handler.Handle(command, ct);
        if (status.IsFailure)
        {
            _logger.Error("{Context}. Error: {ErrorMessage}.", Context, status.Error.ErrorText);
            return status;
        }

        _logger.Information(
            "{Context}. Saved action record. ID: {Id}. Invoker ID: {InvokerId}.",
            Context,
            status.Value.Id,
            status.Value.InvokerId
        );
        return status;
    }
}
