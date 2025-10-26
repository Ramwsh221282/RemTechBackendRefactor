using Cleaners.Adapter.Storage.DataModels;
using Dapper.FluentMap.Mapping;

namespace Cleaners.Adapter.Storage.Configurations;

public sealed class CleanerEntityTypeMap : EntityMap<CleanerDataModel>
{
    public CleanerEntityTypeMap()
    {
        Map(c => c.Id).ToColumn("id");
        Map(c => c.CleanedAmount).ToColumn("cleaned_amount");
        Map(c => c.LastRun).ToColumn("last_run");
        Map(c => c.NextRun).ToColumn("next_run");
        Map(c => c.WaitDays).ToColumn("wait_days");
        Map(c => c.State).ToColumn("state");
        Map(c => c.Hours).ToColumn("hours");
        Map(c => c.Minutes).ToColumn("minutes");
        Map(c => c.Seconds).ToColumn("seconds");
        Map(c => c.ItemsDateDayThreshold).ToColumn("items_date_day_threshold");
    }
}
