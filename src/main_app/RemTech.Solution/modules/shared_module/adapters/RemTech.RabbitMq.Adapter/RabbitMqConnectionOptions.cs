using RemTech.Configuration.Library;

namespace RemTech.RabbitMq.Adapter;

public sealed class RabbitMqConnectionOptions
{
    public string HostName { get; }
    public string UserName { get; }
    public string Password { get; }
    public string Port { get; }

    public RabbitMqConnectionOptions(string filePath)
    {
        IConfig config = new JsonConfig(filePath);
        IConfigCursor cursor = config.Cursor(nameof(RabbitMqConnectionOptions));
        HostName = cursor.GetOption(nameof(HostName));
        UserName = cursor.GetOption(nameof(UserName));
        Password = cursor.GetOption(nameof(Password));
        Port = cursor.GetOption(nameof(Port));
    }
}