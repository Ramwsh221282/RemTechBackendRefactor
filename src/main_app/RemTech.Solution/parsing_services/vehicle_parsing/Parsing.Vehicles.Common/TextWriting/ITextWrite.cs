namespace Parsing.Vehicles.Common.TextWriting;

public interface ITextWrite : IDisposable, IAsyncDisposable
{
    Task<bool> WriteAsync(string text);
}