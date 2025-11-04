using Mailing.Domain.PostedMessages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mailing.Adapters.Storage.PostedMessages;

internal sealed class DbPostedMessage(IPostedMessage Message) : PostedMessageEnvelope(Message)
{
    internal Guid Id { get; } = Message.Data.Id;
    internal Guid PostedBy { get; } = Message.Data.PostedBy;
    internal string Body { get; } = Message.Data.Body;
    internal string Subject { get; } = Message.Data.Subject;
    internal string RecipientAddress { get; } = Message.Data.RecipientAddress;
    internal DateTime CreatedOn { get; } = Message.Data.CreatedOn;

    internal static void Configure(EntityTypeBuilder<DbPostedMessage> builder)
    {
        builder.ToTable("posted_messages");
        builder.HasKey(x => x.Id).HasName("pk_posted_messages");
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.Property(x => x.PostedBy).HasColumnName("posted_by").IsRequired();
        builder.Property(x => x.Body).HasColumnName("body").IsRequired();
        builder.Property(x => x.Subject).HasColumnName("subject").IsRequired().HasMaxLength(255);
        builder.Property(x => x.RecipientAddress).HasColumnName("recipient_address").HasMaxLength(255);
        builder.Property(x => x.CreatedOn).HasColumnName("created_on").IsRequired();
    }
}