using Mailing.Domain.PostedMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.PostedMessages;

internal sealed class DbPostedMessage(IPostedMessage message) : PostedMessageEnvelope(message)
{
}