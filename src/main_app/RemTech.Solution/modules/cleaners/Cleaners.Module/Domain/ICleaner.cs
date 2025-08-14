namespace Cleaners.Module.Domain;

internal interface ICleaner
{
    ICleaner StartWork();
    ICleaner StopWork();
    ICleaner StopWork(long totalElapsedSeconds);
    ICleaner ChangeItemsToCleanThreshold(int threshold);
    ICleaner StartWait();
    ICleaner CleanItem();
    ICleaner ChangeWaitDays(int waitDays);
    CleanerOutput ProduceOutput();
}
