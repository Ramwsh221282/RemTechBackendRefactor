using Mailing.Tests.CleanWriteTests.Contracts;
using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation;

public abstract class WritableInteractorPostmanEnvelope(IWritePostmanCommand origin) :
    IWritePostmanInteractorCommand
{
    public virtual void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics) =>
        origin.Execute(metadata, statistics);
}