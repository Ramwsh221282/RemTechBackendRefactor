namespace Mailing.Module.Domain.Models.ValueObjects;

internal readonly struct Statistics(int sendLimit, int currentSend)
{
    private readonly int _sendLimit = sendLimit;
    private readonly int _currentSend = currentSend;

    public Statistics() : this(0, 0)
    {
    }
}