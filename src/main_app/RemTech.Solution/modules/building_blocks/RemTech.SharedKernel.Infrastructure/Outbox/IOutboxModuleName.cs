namespace RemTech.SharedKernel.Infrastructure.Outbox;

public interface IOutboxModuleName
{
    string DatabaseSchema { get; }
}