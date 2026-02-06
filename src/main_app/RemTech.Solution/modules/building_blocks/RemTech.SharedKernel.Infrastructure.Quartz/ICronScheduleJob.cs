using Quartz;

namespace RemTech.SharedKernel.Infrastructure.Quartz;

/// <summary>
/// Интерфейс для задания с расписанием Cron.
/// </summary>
public interface ICronScheduleJob : IJob;

/// <summary>
/// Атрибут для задания расписания Cron на классах заданий.
/// </summary>
/// <param name="schedule">Расписание Cron.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CronScheduleAttribute(string schedule) : Attribute
{
	/// <summary>
	/// Расписание Cron.
	/// </summary>
	public string Schedule { get; } = schedule;
}
