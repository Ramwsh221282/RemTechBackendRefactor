namespace Parsing.Cache;

public interface IDisabledScraperTracker
{
    Task<bool> HasBeenDisabled(string name, string type);
}
