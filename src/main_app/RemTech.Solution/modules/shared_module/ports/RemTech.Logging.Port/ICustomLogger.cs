namespace RemTech.Logging.Library;

public interface ICustomLogger
{
    public void Info(string template, params object[] arguments);
    public void Warn(string template, params object[] arguments);
    public void Error(string template, params object[] arguments);
    public void Fatal(string template, params object[] arguments);
}
