namespace Mailers.Application.Features;

public sealed record MailersEncryptOptions
{
    public string Key { get; set; } = string.Empty;
}