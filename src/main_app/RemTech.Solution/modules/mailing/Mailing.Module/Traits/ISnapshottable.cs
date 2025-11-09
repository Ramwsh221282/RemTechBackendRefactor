namespace Mailing.Module.Traits;

public interface ISnapshottable<out TSnapshot> where TSnapshot : Snapshot
{
    TSnapshot Snapshotted();
}