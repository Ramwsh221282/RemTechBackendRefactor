namespace Parsing.Vehicles.Common.TextWriting;

public sealed class TextWrite : ITextWrite
{
    private readonly string _path;
    private StreamWriter? _writer;

    public TextWrite(string path)
    {
        _path = path;
    }

    public TextWrite WithDirectory(string directory)
    {
        string newPath = Path.Combine(_path, directory);
        if (!Directory.Exists(newPath))
            Directory.CreateDirectory(newPath);
        return new TextWrite(newPath);
    }

    public TextWrite WithTextFile(string filePath)
    {
        string newPath = filePath.Contains(".txt", StringComparison.OrdinalIgnoreCase) ?
            Path.Combine(_path, filePath)
            : Path.Combine(_path, filePath + ".txt");
        if (File.Exists(newPath))
            File.Delete(newPath);
        return new TextWrite(newPath);
    }

    public void Dispose()
    {
        _writer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_writer == null)
            return;
        await _writer.DisposeAsync();
    }

    public async Task<bool> WriteAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
        if (string.IsNullOrWhiteSpace(_path))
            return false;
        if (_writer == null)
            _writer = new StreamWriter(_path);
        await _writer.WriteLineAsync(text);
        return true;
    }
}