using System.Text.Json;
using GeoLocations.Module.Features.Querying;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RemTech.ContainedItems.Module.Features.MessageBus;
using RemTech.Spares.Module.Features.SinkSpare.Exceptions;
using RemTech.Spares.Module.Features.SinkSpare.Models;
using Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace RemTech.Spares.Module.Features.SinkSpare;

internal sealed class SpareSinkEntrance(
    IGeoLocationQueryService geoLocationQuery,
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    IAddContainedItemsPublisher contained,
    Serilog.ILogger logger,
    IIncreaseProcessedPublisher publisher,
    ConnectionFactory connectionFactory
) : BackgroundService
{
    private const string Exchange = "advertisements";
    private const string Queue = "spares";
    private IConnection? _connection;
    private IChannel? _channel;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await _channel.ExchangeDeclareAsync(
            Exchange,
            ExchangeType.Direct,
            false,
            false,
            cancellationToken: cancellationToken
        );
        await _channel.QueueDeclareAsync(
            Queue,
            false,
            false,
            false,
            cancellationToken: cancellationToken
        );
        await _channel.QueueBindAsync(Queue, Exchange, Queue, cancellationToken: cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null)
            throw new ApplicationException("Spares channel is null.");
        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += Handle;
        await _channel.BasicConsumeAsync(Queue, false, consumer, stoppingToken);
        stoppingToken.ThrowIfCancellationRequested();
    }

    private async Task Handle(object sender, BasicDeliverEventArgs ea)
    {
        if (_channel == null)
            throw new ApplicationException("Spares channel is null.");
        SpareSinkMessage? message = JsonSerializer.Deserialize<SpareSinkMessage>(ea.Body.ToArray());
        if (message != null)
        {
            GeoLocationInfo geoInfo = await geoLocationQuery.VectorSearch(
                message.Spare.LocationText
            );
            SpareLocation location = new SpareLocation(
                geoInfo.Region.Id,
                geoInfo.City.Id,
                geoInfo.Region.Name,
                geoInfo.Region.Type,
                geoInfo.City.Name
            );
            SpareToPersist spare = new(message, location, dataSource, generator);
            try
            {
                await spare.Store();
                LogSaved(message);
                await publisher.SendIncreaseProcessed(
                    message.Parser.ParserName,
                    message.Parser.ParserType,
                    message.Link.LinkName
                );
                await contained.Publish(
                    new AddContainedItemMessage(
                        message.Spare.Id,
                        message.Parser.ParserType,
                        message.Parser.ParserDomain,
                        message.Spare.SourceUrl
                    )
                );
            }
            catch (SpareEmbeddingSourceEmptyValueException ex)
            {
                LogError(ex);
            }
            catch (SpareEmbeddingStringEmptyException ex)
            {
                LogError(ex);
            }
            catch (SpareJsonObjectModifierValueEmptyException ex)
            {
                LogError(ex);
            }
            catch (SparePersistanceCommandEmptyValueException ex)
            {
                LogError(ex);
            }
            catch (SparePersistDuplicateIdException ex)
            {
                LogError(ex);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            finally
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        }
    }

    private void LogSaved(SpareSinkMessage message)
    {
        logger.Information(
            "Сохранена запчасть: {Id} {Title}",
            message.Spare.Id,
            message.Spare.Title
        );
    }

    private void LogError(Exception ex)
    {
        logger.Error("{Entrance}. {Error}.", nameof(SpareSinkEntrance), ex.Message);
    }
}
