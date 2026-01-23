namespace ContainedItems.Worker;

public class Worker : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested) { }
	}
}
