using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class AggregatedConsumersHostedService(
	Serilog.ILogger logger,
	IEnumerable<IConsumer> consumers,
	RabbitMqConnectionSource connectionSource
) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<AggregatedConsumersHostedService>();
	private IEnumerable<IConsumer> Consumers { get; } = consumers;
	private RabbitMqConnectionSource ConnectionSource { get; } = connectionSource;

	private IConnection? _connection;
	private IConnection Connection =>
		_connection ?? throw new InvalidOperationException("Connection is not initialized.");

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		Logger.Information("Shutting down parsers control event listeners.");

		IEnumerable<Task> shutdownTasks = Consumers.Select(async c =>
		{
			try
			{
				await c.Shutdown(cancellationToken);
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "Error while shutting down consumer {ConsumerType}", c.GetType().FullName);
			}
		});

		await Task.WhenAll(shutdownTasks);

		try
		{
			Logger.Information("Closing RabbitMQ connection.");
			Connection.Dispose();
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Error while closing RabbitMQ connection.");
		}

		Logger.Information("Parsers control event listeners shut down.");
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		Logger.Information("Initializing aggregated consumers.");
		_connection = await ConnectionSource.GetConnection(stoppingToken);

		await InitializeConsumers(Consumers, _connection, Logger, stoppingToken);
		await StartConsuming(Consumers, Logger, stoppingToken);
	}

	private static async Task InitializeConsumers(
		IEnumerable<IConsumer> consumers,
		IConnection connection,
		Serilog.ILogger logger,
		CancellationToken ct
	)
	{
		IEnumerable<Task> initTasks = consumers.Select(async c =>
		{
			try
			{
				await c.InitializeChannel(connection, ct);
				logger.Information("Initialized consumer {ConsumerType}", c.GetType().FullName);
			}
			catch (OperationCanceledException) when (ct.IsCancellationRequested)
			{
				logger.Warning("Initialization of consumer {ConsumerType} was canceled.", c.GetType().FullName);
				throw;
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Failed to initialize consumer {ConsumerType}", c.GetType().FullName);
				throw;
			}
		});

		await Task.WhenAll(initTasks);
		logger.Information("Consumers initialized.");
	}

	private static async Task StartConsuming(
		IEnumerable<IConsumer> consumers,
		Serilog.ILogger logger,
		CancellationToken ct
	)
	{
		IEnumerable<Task> startTasks = consumers.Select(async c =>
		{
			try
			{
				await c.StartConsuming(ct);
				logger.Information("Started consuming for {ConsumerType}", c.GetType().FullName);
			}
			catch (OperationCanceledException) when (ct.IsCancellationRequested)
			{
				logger.Warning("StartConsuming for {ConsumerType} was canceled before start.", c.GetType().FullName);
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Failed to start consuming for {ConsumerType}", c.GetType().FullName);
				throw;
			}
		});

		await Task.WhenAll(startTasks);
		logger.Information("Consumers started.");
	}
}
