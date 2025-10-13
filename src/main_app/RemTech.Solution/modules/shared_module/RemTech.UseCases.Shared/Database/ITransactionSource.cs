namespace RemTech.UseCases.Shared.Database;

public interface ITransactionSource : IDisposable, IAsyncDisposable
{
    Task<ITransactionScope> BeginTransactionScope(CancellationToken cancellationToken = default);
}
