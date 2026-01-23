using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Infrastructure.Tokens.BackgroundServices;

public sealed class ExpiredTokensCleanerService(Serilog.ILogger logger, IServiceProvider services) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<ExpiredTokensCleanerService>();
	private IServiceProvider Services { get; } = services;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Execute(stoppingToken);
			await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
		}
	}

	private async Task Execute(CancellationToken ct)
	{
		try
		{
			await using AsyncServiceScope scope = Services.CreateAsyncScope();
			IAccessTokensRepository repository = scope.ServiceProvider.GetRequiredService<IAccessTokensRepository>();
			AccessToken[] expired = (await repository.GetExpired(withLock: true, ct: ct)).ToArray();
			if (expired.Length == 0)
			{
				Logger.Information("No expired tokens found.");
				return;
			}

			await repository.Remove(expired, ct);
			Logger.Information("Expired tokens cleaned. {Count}", expired.Length);
		}
		catch (Exception ex)
		{
			Logger.Fatal(ex, "Error cleaning expired tokens.");
		}
	}
}
