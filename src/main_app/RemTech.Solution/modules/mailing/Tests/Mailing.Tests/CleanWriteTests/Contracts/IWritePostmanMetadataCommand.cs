namespace Mailing.Tests.CleanWriteTests.Contracts;

public interface IWritePostmanMetadataCommand
{
    void Execute(in Guid id, in string email, in string password);
}