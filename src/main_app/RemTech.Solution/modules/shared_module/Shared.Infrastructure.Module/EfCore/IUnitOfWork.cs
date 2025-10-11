using Microsoft.EntityFrameworkCore;
using RemTech.Core.Shared.Result;

namespace Shared.Infrastructure.Module.EfCore;

public interface IUnitOfWork
{
    Task<Status> Save(CancellationToken ct = default);
}

public interface IUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext;
