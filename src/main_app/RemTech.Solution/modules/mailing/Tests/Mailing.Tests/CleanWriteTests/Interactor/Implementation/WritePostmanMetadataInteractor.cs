using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Interactor.Implementation;

public sealed class WritePostmanMetadataInteractor(
    IWritePostmanMetadataCommand driven,
    IWritePostmanMetadataCommand driving)
    : IWritePostmanMetadataInteractorCommand
{
    public void Execute(in Guid id, in string email, in string password)
    {
        driven.Execute(id, email, password);
        driving.Execute(id, email, password);
    }

    private void CallDriven(in Guid id, in string email, in string password) =>
        driven.Execute(in id, in email, in password);

    private void CallDriving(in Guid id, in string email, in string password) =>
        driving.Execute(in id, email, password);
}