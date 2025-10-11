using RemTech.Core.Shared.Result;

namespace RemTech.UseCases.Shared.Cqrs;

public interface IBCommandHandler<in TCommand>
    where TCommand : IBCommand
{
    Task<Status> Handle(TCommand command, CancellationToken ct = default);
}

public interface IBCommandHandler<in TCommand, TResult>
    where TCommand : IBCommand<TResult>
{
    Task<Status<TResult>> Handle(TCommand command, CancellationToken ct = default);
}
