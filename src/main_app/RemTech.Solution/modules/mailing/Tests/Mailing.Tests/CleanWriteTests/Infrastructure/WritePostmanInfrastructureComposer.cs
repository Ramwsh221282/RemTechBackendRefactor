using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Infrastructure;

public sealed class WritePostmanInfrastructureComposer : IWritePostmanInfrastructureCommand
{
    private readonly Queue<IWritePostmanInfrastructureCommand> _queue = [];

    public WritePostmanInfrastructureComposer Add(IWritePostmanInfrastructureCommand command)
    {
        _queue.Enqueue(command);
        return this;
    }

    public void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        while (_queue.Count > 0)
            _queue.Dequeue().Execute(metadata, statistics);
    }
}