namespace RemTech.Configuration.Library;

public interface IConfig
{
    public IConfigCursor Cursor(string key);
}