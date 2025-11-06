using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation.Decorators;

public sealed class WritableExceptionHandledPostman(IWritePostmanInteractorCommand origin)
    : WritableInteractorPostmanEnvelope(origin)
{
    public override void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        try
        {
            base.Execute(metadata, statistics);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}