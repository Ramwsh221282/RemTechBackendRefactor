using Quartz;

namespace RemTech.SharedKernel.Infrastructure.Quartz;

public interface ICronScheduleJob : IJob;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CronScheduleAttribute(string schedule) : Attribute
{
    public string Schedule { get; } = schedule;
}
