namespace ContainedItems.Worker;

public class BackgroundWorker : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested) { }
	}
}
