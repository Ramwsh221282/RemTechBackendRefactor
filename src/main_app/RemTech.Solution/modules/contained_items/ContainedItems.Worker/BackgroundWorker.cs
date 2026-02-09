namespace ContainedItems.Worker;

/// <summary>
/// Фоновый рабочий процесс для модуля содержащихся элементов.
/// </summary>
public class BackgroundWorker : BackgroundService
{
	/// <summary>
	/// Выполняет асинхронную работу фонового сервиса.
	/// </summary>
	/// <param name="stoppingToken">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию выполнения.</returns>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}
	}
}
