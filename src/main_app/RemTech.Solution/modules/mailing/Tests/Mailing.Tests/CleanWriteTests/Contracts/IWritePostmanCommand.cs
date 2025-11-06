using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Contracts;

public interface IWritePostmanCommand
{
    void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics);
}