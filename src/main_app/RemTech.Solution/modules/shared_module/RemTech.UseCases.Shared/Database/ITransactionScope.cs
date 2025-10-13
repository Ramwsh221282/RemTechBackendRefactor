namespace RemTech.UseCases.Shared.Database;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    Task<Result.Pattern.Result> Commit(CancellationToken ct = default);
}
