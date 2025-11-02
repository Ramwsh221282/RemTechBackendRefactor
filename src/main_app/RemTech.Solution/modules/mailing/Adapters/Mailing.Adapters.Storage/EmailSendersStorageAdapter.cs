using Mailing.Domain.EmailSendingContext;
using Mailing.Domain.EmailSendingContext.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Adapters.Storage;

internal sealed class StoredEmailSender
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly string _service;
    private readonly string _password;
    private readonly int _sendLimit;
    private readonly int _currentSent;

    internal static StoredEmailSender From(EmailSender sender) => sender.ProjectTo(SinkMethod);

    internal static StoredEmailSender Empty => new();

    internal static EmailSenderDataSink<StoredEmailSender> SinkMethod =>
        (id, email, service, password, limit, current) =>
            new StoredEmailSender(id, email, service, password, limit, current);

    private StoredEmailSender(Guid id,
        string email,
        string service,
        string password,
        int limit,
        int currentSended
    )
    {
        _id = id;
        _email = email;
        _service = service;
        _password = password;
        _sendLimit = limit;
        _currentSent = currentSended;
    }

    internal StoredEmailSender()
    {
        _id = Guid.NewGuid();
        _email = string.Empty;
        _service = string.Empty;
        _password = string.Empty;
        _sendLimit = 0;
        _currentSent = 0;
    }

    internal static EntityTypeBuilder<StoredEmailSender> Configure(EntityTypeBuilder<StoredEmailSender> builder)
    {
        builder.ToTable("email_senders");
        builder.HasKey(s => s._id).HasName("pk_senders");
        builder.Property(s => s._id).HasColumnName("id").IsRequired();
        builder.Property(s => s._service).HasColumnName("service").IsRequired().HasMaxLength(100);
        builder.Property(s => s._password).HasColumnName("password").IsRequired();
        builder.Property(s => s._sendLimit).HasColumnName("send_limit").IsRequired();
        builder.Property(s => s._currentSent).HasColumnName("current_sent").IsRequired();
        return builder;
    }
}

internal sealed class EmailSenderDataModelConfiguration : IEntityTypeConfiguration<StoredEmailSender>
{
    public void Configure(EntityTypeBuilder<StoredEmailSender> builder) => StoredEmailSender.Configure(builder);
}

public sealed class EmailSendersStorageAdapter(PostgresDatabase database)
{
}