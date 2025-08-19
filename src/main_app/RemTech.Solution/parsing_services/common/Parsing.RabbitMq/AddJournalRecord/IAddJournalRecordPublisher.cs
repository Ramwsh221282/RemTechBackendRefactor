namespace Parsing.RabbitMq.AddJournalRecord;

public interface IAddJournalRecordPublisher
{
    Task PublishJournalRecord(string parserName, string parserType, string action, string text);
}
