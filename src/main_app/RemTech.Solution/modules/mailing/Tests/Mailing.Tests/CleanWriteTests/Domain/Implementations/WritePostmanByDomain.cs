using Mailing.Tests.CleanWriteTests.Contracts;
using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Domain.Implementations;

public sealed class WritePostmanByDomain(IWritePostmanCommand? above = null) : IWritePostmanDomainCommand
{
    private readonly WritePostmanMetadataDomainCommand _meta = new();
    private readonly WritePostmanStatisticsDomain _stats = new();

    public void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        metadata.Write(_meta);
        statistics.Write(_stats);
        above?.Execute(metadata, statistics);
    }
}