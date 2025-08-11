namespace Shared.Infrastructure.Module.Cqrs;

public interface ICommand { }

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken ct = default);
}
