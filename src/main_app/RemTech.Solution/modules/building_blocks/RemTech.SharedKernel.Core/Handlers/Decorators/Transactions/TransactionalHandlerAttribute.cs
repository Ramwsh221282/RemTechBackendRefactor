namespace RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

/// <summary>
/// Атрибут, указывающий, что обработчик должен выполняться в транзакции. Используйте, если требуется транзакционное поведение.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TransactionalHandlerAttribute : Attribute;
