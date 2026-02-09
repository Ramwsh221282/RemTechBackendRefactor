namespace RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

/// <summary>
/// Интерфейс валидирующего обработчика команд.
/// </summary>
/// <typeparam name="TCommand">Тип команды.</typeparam>
/// <typeparam name="TResult">Тип результата.</typeparam>
public interface IValidatingCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
	where TCommand : ICommand;
