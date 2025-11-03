using System.Data;
using Dapper;
using Mailing.Domain.EmailSendingContext;
using Mailing.Domain.EmailSendingContext.Events;
using Mailing.Domain.EmailSendingContext.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
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

    private static EmailSenderDataSink<StoredEmailSender> SinkMethod =>
        ((id, email, service, password, limit, sent, port) =>
            new StoredEmailSender(id, email, service, password, limit, sent));

    internal StoredEmailSender(
        Guid id,
        string email,
        string service,
        string password,
        int limit,
        int currentSent
    ) =>
        (_id, _email, _service, _password, _sendLimit, _currentSent) =
        (id, email, service, password, limit, currentSent);


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
        builder.Property(s => s._email).HasColumnName("email").IsRequired().HasMaxLength(255);
        builder.Property(s => s._service).HasColumnName("service").IsRequired().HasMaxLength(100);
        builder.Property(s => s._password).HasColumnName("password").IsRequired();
        builder.Property(s => s._sendLimit).HasColumnName("send_limit").IsRequired();
        builder.Property(s => s._currentSent).HasColumnName("current_sent").IsRequired();
        return builder;
    }
}

internal sealed class EmailSenderDataModelConfiguration
    : IEntityTypeConfiguration<StoredEmailSender>
{
    public void Configure(EntityTypeBuilder<StoredEmailSender> builder) =>
        StoredEmailSender.Configure(builder);
}

internal sealed class SentMessageDataModel
{
    private readonly Guid _id;
    private readonly Guid _senderId;
    private readonly string _recipient;
    private readonly string _subject;
    private readonly string _body;
    private readonly DateTime _created;

    internal SentMessageDataModel(
        Guid id,
        Guid senderId,
        string recipient,
        string subject,
        string body,
        DateTime created) =>
        (_id, _senderId, _recipient, _subject, _body, _created) =
        (id, senderId, recipient, subject, body, created);

    internal static void Configure(EntityTypeBuilder<SentMessageDataModel> builder)
    {
        builder.ToTable("sent_messages");
        builder.HasKey(m => m._id).HasName("pk_sender_id");
        builder.Property(m => m._id).HasColumnName("id").IsRequired();
        builder.Property(m => m._senderId).HasColumnName("sender_id").IsRequired();
        builder.Property(m => m._body).HasColumnName("body").IsRequired().HasMaxLength(512);
        builder.Property(m => m._recipient).HasColumnName("recipient").IsRequired().HasMaxLength(255);
        builder.Property(m => m._created).HasColumnName("created").IsRequired();
        builder.Property(m => m._subject).HasColumnName("subject").IsRequired().HasMaxLength(255);
    }
}

internal sealed class SentMessageEntityTypeConfiguration : IEntityTypeConfiguration<SentMessageDataModel>
{
    public void Configure(EntityTypeBuilder<SentMessageDataModel> builder) =>
        SentMessageDataModel.Configure(builder);
}

internal sealed class EmailSenderCreatedEventHandler(
    PostgresDatabase db,
    Serilog.ILogger logger
)
    : IDomainEventHandler<EmailSenderCreated>
{
    private const string Context = nameof(EmailSenderCreated);

    public async Task<Status> Handle(EmailSenderCreated @event, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO mailing_module.email_senders
                           (id, service, password, send_limit, current_sent, email)
                           VALUES
                           (@id, @service, @password, @send_limit, @current_sent, @email)
                           """;

        var parameters = FoldToParameters(@event);
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);

        using var connection = await db.ProvideConnection(ct);

        try
        {
            await connection.ExecuteAsync(command);
            var status = Status.Success();
            logger.Information("{Context} email sender has been inserted.", Context);
            return status;
        }
        catch (Exception ex)
        {
            logger.Error("{Context} exception: {Ex}.", Context, ex.Message);
            return Status.Internal("Не удается сохранить данные почтового сервиса.");
        }
    }

    private static DynamicParameters FoldToParameters(EmailSenderCreated @event)
    {
        DynamicParameters parameters = new();
        @event.Fold(((id, email, service, password, limit, sent, port) =>
        {
            parameters.Add("@id", id, DbType.Guid);
            parameters.Add("@service", service, DbType.String);
            parameters.Add("@password", password, DbType.String);
            parameters.Add("@send_limit", limit, DbType.Int32);
            parameters.Add("@current_sent", sent, DbType.Int32);
            parameters.Add("@email", email, DbType.String);
        }));
        return parameters;
    }
}

internal sealed class EmailMessageSentEventHandler(PostgresDatabase database) : IDomainEventHandler<EmailMessageSent>
{
    public async Task<Status> Handle(EmailMessageSent @event, CancellationToken ct = default)
    {
        // private readonly Guid _id;
        // private readonly string _email;
        // private readonly string _service;
        // private readonly string _password;
        // private readonly int _sendLimit;
        // private readonly int _currentSent;

        var senderModel = @event.Fold((sender =>
        {
            sender.Fold<StoredEmailSender>((id, email, service, password, sendLimit, current, _) =>
                new StoredEmailSender(id, email, service, password, sendLimit, current));
        }));
    }
}