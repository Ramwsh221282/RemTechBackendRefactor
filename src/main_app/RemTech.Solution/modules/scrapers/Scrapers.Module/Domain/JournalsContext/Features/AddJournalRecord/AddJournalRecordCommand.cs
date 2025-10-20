using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using RemTech.Core.Shared.Cqrs;

namespace Scrapers.Module.Domain.JournalsContext.Features.AddJournalRecord;

internal sealed record AddJournalRecordCommand(
    string ParserName,
    string ParserType,
    string Action,
    string Text
) : ICommand
{
    public static AddJournalRecordCommand FromEventArgs(BasicDeliverEventArgs ea)
    {
        ReadOnlyMemory<byte> bytes = ea.Body;
        string json = Encoding.UTF8.GetString(bytes.Span);
        JsonDocument document = JsonDocument.Parse(json);
        string? parserName = document.RootElement.GetProperty("parserName").GetString();
        string? parserType = document.RootElement.GetProperty("parserType").GetString();
        string? action = document.RootElement.GetProperty("action").GetString();
        string? text = document.RootElement.GetProperty("text").GetString();
        document.Dispose();
        if (string.IsNullOrWhiteSpace(parserName))
            throw new InvalidOperationException(
                $"{nameof(AddJournalRecordCommand)} Parser name not provided."
            );
        if (string.IsNullOrWhiteSpace(parserType))
            throw new InvalidOperationException(
                $"{nameof(AddJournalRecordCommand)} Parser type not provided."
            );
        if (string.IsNullOrWhiteSpace(action))
            throw new InvalidOperationException(
                $"{nameof(AddJournalRecordCommand)} Action not provided."
            );
        if (string.IsNullOrWhiteSpace(text))
            throw new InvalidOperationException(
                $"{nameof(AddJournalRecordCommand)} Text not provided."
            );
        return new AddJournalRecordCommand(parserName, parserType, action, text);
    }
}
