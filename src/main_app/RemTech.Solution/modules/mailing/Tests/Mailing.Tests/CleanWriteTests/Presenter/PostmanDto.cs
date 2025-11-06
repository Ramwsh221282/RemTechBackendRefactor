namespace Mailing.Tests.CleanWriteTests.Presenter;

public sealed class PostmanDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Limit { get; set; } = 0;
    public int CurrentSend { get; set; } = 0;
}