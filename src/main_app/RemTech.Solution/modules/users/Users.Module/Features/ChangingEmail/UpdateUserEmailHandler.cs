﻿using Mailing.Module.Bus;
using Mailing.Module.Public;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.CommonAbstractions;
using Users.Module.Features.ChangingEmail.Exceptions;
using Users.Module.Features.ChangingEmail.Shared;

namespace Users.Module.Features.ChangingEmail;

internal sealed class UpdateUserEmailHandler(
    NpgsqlDataSource dataSource,
    MailingBusPublisher publisher,
    Serilog.ILogger logger,
    ConnectionMultiplexer multiplexer,
    HasSenderApi hasSenderApi,
    FrontendUrl frontendUrl,
    ConfirmationEmailsCache cache
) : ICommandHandler<UpdateUserEmailCommand, UpdateUserEmailResponse>
{
    public async Task<UpdateUserEmailResponse> Handle(
        UpdateUserEmailCommand command,
        CancellationToken ct = default
    )
    {
        new EmailValidation().ValidateEmail(command.NewEmail);
        if (!await hasSenderApi.HasSender())
            throw new SendersAreNotAvailableYetException();
        if (!await IsAuthorized(command))
            throw new UnauthorizedAccessException();
        await new ChangeEmailTransaction(dataSource).Execute(command.NewEmail, command.UserId, ct);
        Guid confirmationKey = await new EmailConfirmationKeyGeneration(cache).Generate(
            command.UserId
        );
        await new EmailChangeMailingMessage(frontendUrl, confirmationKey, publisher).Send(
            command.NewEmail,
            ct
        );
        logger.Information(
            "User {Guid} has updated email by self {Email}.",
            command.UserId,
            command.NewEmail
        );
        return new UpdateUserEmailResponse(command.UserId, command.NewEmail);
    }

    private async Task<bool> IsAuthorized(UpdateUserEmailCommand command)
    {
        string inputPassword = command.InputPassword;
        string realPassword = command.UserJwt.MakeOutput().Password;
        if (BCrypt.Net.BCrypt.Verify(inputPassword, realPassword))
            return true;
        await command.UserJwt.Deleted(multiplexer);
        return false;
    }
}
