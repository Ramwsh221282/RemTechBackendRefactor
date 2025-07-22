namespace Parsing.SDK.Logging;

public interface IParsingLog
{
    void Info(string info, params object[] args);
    void Warning(string info, params object[] args);
    void Error(string info, params object[] args);
}