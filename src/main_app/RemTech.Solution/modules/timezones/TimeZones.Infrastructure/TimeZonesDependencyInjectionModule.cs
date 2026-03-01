using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Core.Handlers;
using Timezones.Core.Contracts;
using Timezones.Core.Models;
using TimeZones.Infrastructure.Persistence;
using TimeZones.Infrastructure.Queries;

namespace TimeZones.Infrastructure;

public static class TimeZonesDependencyInjectionModule
{
	extension(IServiceCollection services)
	{
		public void InjectTimeZonesModule()
		{
			services.AddOptions<TimeZonesProviderOptions>().BindConfiguration(nameof(TimeZonesProviderOptions));
			services.TryAddSingleton<ITimeZonesProvider, TimeZonesProvider>();
			services.TryAddSingleton<IRegionDateTimesRepository, CachedRegionDateTimesRepository>();
			services.TryAddScoped<
				IQueryHandler<GetRegionLocalDateTimesQuery, IReadOnlyList<RegionLocalDateTime>>,
				GetRegionLocalDateTimesQueryHandler
			>();
			services.AddHostedService<RegionLocalDateTimesCacheUpdateService>();
		}
	}
}
