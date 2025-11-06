using Mailing.Tests.CleanWriteTests.Contracts;

namespace Mailing.Tests.CleanWriteTests.Models;

public sealed class TestPostmanMetadata(Guid id, string email, string password)
{
    public void Write(IWritePostmanMetadataCommand command) =>
        command.Execute(id, email, password);
}