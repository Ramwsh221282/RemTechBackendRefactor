using Microsoft.EntityFrameworkCore;
using RemTech.Result.Pattern;

namespace Shared.Infrastructure.Module.EfCore;

public interface IUnitOfWork
{
    Task<Result> Save(CancellationToken ct = default);
}

public interface IUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext;
