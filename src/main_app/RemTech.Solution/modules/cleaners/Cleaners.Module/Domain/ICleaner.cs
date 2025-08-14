namespace Cleaners.Module.Domain;

internal interface ICleaner
{
    ICleaner StartWork();
    ICleaner StopWork();
    ICleaner StartWait();
    ICleaner CleanItem();
    ICleaner ChangeWaitDays(int waitDays);
    CleanerOutput ProduceOutput();
}
