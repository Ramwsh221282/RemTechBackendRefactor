namespace Parsing.SDK.ScrapingArtifacts;

public interface IScrapingArtifact<T>
{
    Task<T> Read();
}