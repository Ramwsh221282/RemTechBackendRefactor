using Mailing.Tests.CleanWriteTests.Contracts;
using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation;

public sealed class WritePostmanInteractive(IWritePostmanCommand driven, IWritePostmanCommand driving)
    : IWritePostmanInteractorCommand
{
    public void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        driven.Execute(metadata, statistics);
        driving.Execute(metadata, statistics);
    }
}