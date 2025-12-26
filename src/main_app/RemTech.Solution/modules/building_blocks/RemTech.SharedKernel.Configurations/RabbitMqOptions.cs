namespace RemTech.SharedKernel.Configurations;

public class RabbitMqOptions
{
    public string Hostname { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Port { get; set; } = -1;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Hostname))
            throw new ArgumentException("Cannot use RabbitMq Options. Hostname was not set.");
        if (string.IsNullOrWhiteSpace(Username))
            throw new ArgumentException("Cannot use RabbitMq Options. Username was not set.");
        if (string.IsNullOrWhiteSpace(Password))
            throw new ArgumentException("Cannot use RabbitMq Options. Password was not set.");
        if (Port <= 0)
            throw new ArgumentException("Cannot use RabbitMq Options. Port was not set.");
    }
}