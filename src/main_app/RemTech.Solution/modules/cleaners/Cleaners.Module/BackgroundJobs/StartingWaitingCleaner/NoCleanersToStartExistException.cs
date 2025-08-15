namespace Cleaners.Module.BackgroundJobs.StartingWaitingCleaner;

internal sealed class NoCleanersToStartExistException : Exception
{
    public NoCleanersToStartExistException()
        : base("Пока ещё нет чистильщиков готовых начать работу.") { }
}
