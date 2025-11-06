using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation.Decorators;

public sealed class WritableInteractorPostmanWithCallBack(
    Action callback,
    IWritePostmanInteractorCommand origin)
    : WritableInteractorPostmanEnvelope(origin)
{
    public override void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        base.Execute(metadata, statistics);
        callback();
    }
}