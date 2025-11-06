using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class TestPostman(TestPostmanMetadata metadata, TestPostmanStatistics statistics)
{
    public void Write(IWritePostmanCommand command) =>
        command.Execute(metadata, statistics);
}