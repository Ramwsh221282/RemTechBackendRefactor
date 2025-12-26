using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Telemetry.Domain.Models.ValueObjects;
using Telemetry.Domain.UseCases.AddAction.Input;
using Telemetry.Domain.UseCases.AddAction.Output;

namespace Telemetry.Domain.UseCases.AddAction.Handlers;

public sealed class AddActionValidatingHandler
    : ICommandHandler<AddActionCommand, Status<ActionRecordOutput>>
{
    private readonly ICommandHandler<AddActionCommand, Status<ActionRecordOutput>> _handler;

    public AddActionValidatingHandler(
        ICommandHandler<AddActionCommand, Status<ActionRecordOutput>> handler
    ) => _handler = handler;

    public Task<Status<ActionRecordOutput>> Handle(
        AddActionCommand command,
        CancellationToken ct = default
    )
    {
        List<Error> errors = [];
        errors = Validate(errors, command.Name, ActionName.Create);
        errors = Validate(errors, command.Status, ActionStatus.Create);
        errors = Validate(errors, command.InvokerId, ActionInvokerId.Create);
        errors = Validate(errors, command.OccuredAt, ActionDate.Create);
        errors = Validate(errors, command.Comments, ActionComment.Create);
        return (errors.Count == 0) switch
        {
            false => _handler.Handle(command, ct),
            true => Task.FromResult(Status<ActionRecordOutput>.Failure(ValidationError(errors))),
        };
    }

    private static List<Error> Validate<T>(List<Error> errors, T property, Func<T, Status> method)
    {
        Status status = method(property);
        if (status.IsFailure)
            errors.Add(status.Error);
        return errors;
    }

    private static Error ValidationError(List<Error> errors) =>
        Error.Validation(string.Join(" ,", errors.Select(er => er.ErrorText)));
}
