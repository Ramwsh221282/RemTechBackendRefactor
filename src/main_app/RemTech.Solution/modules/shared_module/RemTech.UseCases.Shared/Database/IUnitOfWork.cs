namespace RemTech.UseCases.Shared.Database;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Task<Result.Pattern.Result> SaveChanges(CancellationToken ct = default);
}
