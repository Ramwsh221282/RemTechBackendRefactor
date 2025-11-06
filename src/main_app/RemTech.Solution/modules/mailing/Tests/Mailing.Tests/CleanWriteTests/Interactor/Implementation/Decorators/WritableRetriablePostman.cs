using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation.Decorators;

public sealed class WritableRetriablePostman(int retryCount, IWritePostmanInteractorCommand origin)
    : WritableInteractorPostmanEnvelope(origin)
{
    public override void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        int current = 0;
        while (current < retryCount)
        {
            origin.Execute(metadata, statistics);
            break;
        }
    }
}