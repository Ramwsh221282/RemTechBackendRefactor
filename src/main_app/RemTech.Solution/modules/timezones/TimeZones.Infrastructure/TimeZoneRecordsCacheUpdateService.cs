using Microsoft.Extensions.Hosting;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

internal sealed class TimeZoneRecordsCacheUpdateService : BackgroundService
{
    public TimeZoneRecordsCacheUpdateService(ITimeZonesProvider provider, ITimeZonesRepository repository, Serilog.ILogger logger)
    {
        _provider = provider;
        _repository = repository;
        _logger = logger.ForContext<TimeZoneRecordsCacheUpdateService>();
    }

    private readonly ITimeZonesProvider _provider;
    private readonly ITimeZonesRepository _repository;
    private readonly Serilog.ILogger _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Information("Начинаем обновление кэша часовых поясов");
            try
            {
                await ExecuteUpdate(stoppingToken);
                _logger.Information("Успешно обновили кэш часовых поясов");
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Фатальная ошибка при обновлении кэша часовых поясов");
            }
        }
    }

    private async Task ExecuteUpdate(CancellationToken ct)
    {
        Dictionary<string, TimeZoneRecord> timeZones = await _provider.FetchTimeZones(ct);
        IReadOnlyList<TimeZoneRecord> records = TimeZoneRecord.ToList(timeZones);
        await _repository.Refresh(records);
        await Task.Delay(TimeSpan.FromMinutes(5), ct);
    }
}
