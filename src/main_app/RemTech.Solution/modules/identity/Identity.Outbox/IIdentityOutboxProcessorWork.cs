using RemTech.Outbox.Shared;

namespace Identity.Outbox;

public interface IIdentityOutboxProcessorWork
{
    Task<ProcessedOutboxMessages> ProcessMessages();
}