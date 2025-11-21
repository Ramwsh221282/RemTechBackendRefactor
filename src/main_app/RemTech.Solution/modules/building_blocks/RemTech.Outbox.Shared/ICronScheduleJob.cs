using Quartz;

namespace RemTech.Outbox.Shared;

public interface ICronScheduleJob : IJob;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CronScheduleAttribute(string schedule) : Attribute
{
    public string Schedule { get; } = schedule;
}