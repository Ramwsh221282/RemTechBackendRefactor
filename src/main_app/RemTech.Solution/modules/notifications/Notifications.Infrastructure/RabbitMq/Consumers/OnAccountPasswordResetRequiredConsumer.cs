using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notifications.Core.PendingEmails.Features.AddPendingEmail;
using Notifications.Infrastructure.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Serilog;

namespace Notifications.Infrastructure.RabbitMq.Consumers;

public sealed class OnAccountPasswordResetRequiredConsumer(
    RabbitMqConnectionSource rabbitMq,
    IOptions<FrontendOptions> frontend,
    IServiceProvider services
) : IConsumer
{
    private const string Exchange = "accounts";
    private const string RoutingKey = "reset-password";
    private const string Queue = RoutingKey;

    private IChannel? _channel;
    private RabbitMqConnectionSource RabbitMq { get; } = rabbitMq;
    private Serilog.ILogger Logger { get; } =
        Serilog.Log.ForContext<OnAccountPasswordResetRequiredConsumer>();
    private FrontendOptions FrontendOptions { get; } = frontend.Value;
    private IServiceProvider Services { get; } = services;
    private IChannel Channel => _channel ?? throw new ArgumentException("Channel not initialized");

    public async Task InitializeChannel(IConnection connection, CancellationToken ct = default) =>
        _channel = await TopicConsumerInitialization.InitializeChannel(
            RabbitMq,
            Exchange,
            Queue,
            RoutingKey,
            ct
        );

    public async Task Shutdown(CancellationToken ct = default)
    {
        if (Channel.IsClosed)
            return;
        await Channel.CloseAsync(cancellationToken: ct);
    }

    public async Task StartConsuming(CancellationToken ct = default) { }

    private AsyncEventHandler<BasicDeliverEventArgs> Handler =>
        async (_, ea) =>
        {
            Logger.Information("Received password reset required message.");

            try
            {
                ResetPasswordRequiredMessage message =
                    ResetPasswordRequiredMessage.FromDeliverEventArgs(ea);

                if (!message.IsValid(out string validationError))
                {
                    Logger.Error(
                        "Invalid password reset required message: {Error}",
                        validationError
                    );
                    return;
                }

                string resetUrl = BuildResetPasswordUrl(message);
                Result<Unit> result = await HandleMessage(message, resetUrl);
                if (result.IsFailure)
                {
                    Logger.Error(
                        "Failed to process password reset required message: {Error}",
                        result.Error.Message
                    );
                    return;
                }

                Logger.Information("Processed password reset required message successfully.");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Error processing password reset required message.");
            }
            finally
            {
                await Channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

    private async Task<Result<Unit>> HandleMessage(
        ResetPasswordRequiredMessage message,
        string confirmationUrl
    )
    {
        string body =
            $"Для сброса пароля учетной записи необходимо перейти по ссылке: {confirmationUrl}";
        string subject = "Сброс пароля";
        string recipient = message.AccountEmail;
        AddPendingEmailCommand command = new(Recipient: recipient, Subject: subject, Body: body);
        return await Services.CreatePendingMessage(command);
    }

    private string BuildResetPasswordUrl(ResetPasswordRequiredMessage message)
    {
        FrontendOptions.Validate();
        string frontendUrl = FrontendOptions.Url;
        string ticketId = message.TicketId.ToString();
        return $"{frontendUrl}/api/account/reset-password?ticketId={ticketId}&accountId={message.AccountId}";
    }

    private sealed record ResetPasswordRequiredMessage(
        Guid AccountId,
        string AccountEmail,
        Guid TicketId,
        string TicketPurpose
    )
    {
        public static ResetPasswordRequiredMessage FromDeliverEventArgs(BasicDeliverEventArgs ea) =>
            JsonSerializer.Deserialize<ResetPasswordRequiredMessage>(ea.Body.Span)!;

        public bool IsValid(out string error)
        {
            List<string> errors = [];
            if (AccountId == Guid.Empty)
                errors.Add("Идентификатор учетной записи не указан.");
            if (string.IsNullOrWhiteSpace(AccountEmail))
                errors.Add("Почта учетной записи не указана.");
            if (TicketId == Guid.Empty)
                errors.Add("Идентификатор тикета не указан.");
            if (string.IsNullOrWhiteSpace(TicketPurpose))
                errors.Add("Назначение тикета не указано.");
            error = string.Join(", ", errors);
            return errors.Count == 0;
        }
    }
}
