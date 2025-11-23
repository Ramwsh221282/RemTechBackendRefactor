using Mailing.Application;
using Mailing.Application.Mailers.SendEmail;
using Mailing.Core.Mailers;
using Mailing.Presenters.Shared;
using RemTech.Functional.Extensions;
 
namespace Mailing.Presenters.Mailers.TestEmailSending;

public sealed class TestEmailSendingGateway(
    ICommandHandler<SendEmailCommand, DeliveredMessage> handler)
    : IGateway<TestEmailSendingRequest, TestEmailSendingResponse>
{
    public async Task<Result<TestEmailSendingResponse>> Execute(TestEmailSendingRequest sendingRequest)
    {
        SendEmailCommand command = new(
            TargetEmail: sendingRequest.TargetEmail,
            Subject: "Тестовая отправка сообщения",
            Body: "Тестовое содержание сообщения",
            sendingRequest.Ct,
            SenderId: sendingRequest.SenderId);
        AsyncOperationResult<DeliveredMessage> result = new(() => handler.Execute(command));
        Result<DeliveredMessage> commandResult = await result.Process();
        return commandResult.Map(r => new TestEmailSendingResponse(r.Mailer.Id));
    }
}