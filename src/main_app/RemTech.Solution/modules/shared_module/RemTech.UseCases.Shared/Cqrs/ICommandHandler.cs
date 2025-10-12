using RemTech.Result.Pattern;

namespace RemTech.UseCases.Shared.Cqrs;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result.Pattern.Result> Handle(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken ct = default);
}
