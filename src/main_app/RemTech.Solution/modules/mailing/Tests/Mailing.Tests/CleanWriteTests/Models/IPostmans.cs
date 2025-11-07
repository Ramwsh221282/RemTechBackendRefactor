using RemTech.Core.Shared.Async;

namespace Mailing.Tests.CleanWriteTests.Models;

public interface IPostmans
{
    Future Save(ITestPostman postman, CancellationToken ct = default);
    Future Remove(ITestPostman postman, CancellationToken ct = default);
    FromFuture<ITestPostman> FindById(Guid id, CancellationToken ct = default);
    FromFuture<ITestPostman> FindByEmail(string email, CancellationToken ct = default);
}