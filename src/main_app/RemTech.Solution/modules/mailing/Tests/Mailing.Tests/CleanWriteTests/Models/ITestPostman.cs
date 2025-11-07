using RemTech.Core.Shared.Async;

namespace Mailing.Tests.CleanWriteTests.Models;

public interface ITestPostman
{
    Future Save(IPostmans postmans, CancellationToken ct = default);
    T Transform<T>(Func<PostmanSnapshot, T> transformation);
}