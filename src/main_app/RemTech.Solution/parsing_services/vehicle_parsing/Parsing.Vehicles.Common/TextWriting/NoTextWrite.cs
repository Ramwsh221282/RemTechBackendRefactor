namespace Parsing.Vehicles.Common.TextWriting;

public sealed class NoTextWrite : ITextWrite
{
    public void Dispose()
    {
        
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<bool> WriteAsync(string text)
    {
        return Task.FromResult(false);
    }
}