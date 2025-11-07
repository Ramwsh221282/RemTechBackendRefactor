using RemTech.Core.Shared.Async;

namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class TestPostman(TestPostmanMetadata metadata, TestPostmanStatistics statistics) : ITestPostman
{
    public Future Save(IPostmans postmans, CancellationToken ct = default) =>
        postmans.Save(this);

    public T Transform<T>(Func<PostmanSnapshot, T> transformation) =>
        transformation(MakeSnapshot());

    private PostmanSnapshot MakeSnapshot()
    {
        PostmanSnapshot snapshot = PostmanSnapshot.Empty();
        snapshot = metadata.Supply(snapshot);
        snapshot = statistics.Supply(snapshot);
        return snapshot;
    }

    public TestPostman(TestPostmanMetadata metadata) : this(metadata, new TestPostmanStatistics())
    {
    }
}