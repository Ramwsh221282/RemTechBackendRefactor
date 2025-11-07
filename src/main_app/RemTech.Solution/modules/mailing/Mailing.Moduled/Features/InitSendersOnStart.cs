using System.Data.Common;
using Mailing.Moduled.Cache;
using Mailing.Moduled.Contracts;
using Mailing.Moduled.Models;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Mailing.Moduled.Features;

internal sealed class InitSendersOnStart(
    MailingSendersCache cache,
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource
) : BackgroundService
{
    private const string Sql = "SELECT email, key FROM mailing_module.senders";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
                stoppingToken
            );
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = Sql;
            await using DbDataReader reader = await command.ExecuteReaderAsync(stoppingToken);
            if (!reader.HasRows)
            {
                logger.Warning(
                    "{Entrance}. Application has not senders yet. Not initializing cache.",
                    nameof(InitSendersOnStart)
                );
                return;
            }

            while (await reader.ReadAsync(stoppingToken))
            {
                string email = reader.GetString(0);
                string key = reader.GetString(1);
                IEmailSender sender = new EmailSender(email, key);
                await cache.Add(sender);
                logger.Information("Inited cache sender: {email}", email);
            }
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(InitSendersOnStart), ex.Message);
        }

        stoppingToken.ThrowIfCancellationRequested();
    }
}