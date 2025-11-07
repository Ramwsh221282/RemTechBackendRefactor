using Mailing.Domain.General;
using RemTech.Core.Shared.Async;

namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class EmptyPostman : ITestPostman
{
    public Future Save(IPostmans postmans, CancellationToken ct = default) =>
        throw new NotFoundException("Postman не найден.");

    public T Transform<T>(Func<PostmanSnapshot, T> transformation) =>
        throw new NotFoundException("Postman не найден.");
}