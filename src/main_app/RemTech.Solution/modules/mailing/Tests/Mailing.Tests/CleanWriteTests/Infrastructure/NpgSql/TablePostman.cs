using Mailing.Tests.CleanWriteTests.Models;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

internal sealed class TablePostman
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required int CurrentSent { get; init; }
    public required int CurrentLimit { get; init; }

    public TestPostman ToPostman() => new(
        new TestPostmanMetadata(Id, Email, Password),
        new TestPostmanStatistics(CurrentLimit, CurrentSent));
}