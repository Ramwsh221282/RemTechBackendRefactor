using Cleaners.Module.Domain.Exceptions;

namespace Cleaners.Module.Domain;

internal sealed class Cleaner : ICleaner
{
    private static readonly string[] AllowedStates = ["Работает", "Ожидает", "Отключен"];
    private readonly Guid _id;
    private readonly int _cleanedAmount;
    private readonly DateTime _lastRun;
    private readonly DateTime _nextRun;
    private readonly int _waitDays;
    private readonly string _state;
    private readonly int _hours;
    private readonly int _minutes;
    private readonly int _seconds;

    public ICleaner StartWork()
    {
        if (_state == "Работает")
            throw new CleanerIsAlreadyBusyException();
        int newCleanedAmount = 0;
        string newState = "Работает";
        DateTime newLastRun = DateTime.UtcNow;
        DateTime newNextRun = DateTime.UtcNow.AddDays(_waitDays);
        int newHours = 0;
        int newMinutes = 0;
        int newSeconds = 0;
        return Create(
            _id,
            newCleanedAmount,
            newLastRun,
            newNextRun,
            _waitDays,
            newState,
            newHours,
            newMinutes,
            newSeconds
        );
    }

    public ICleaner StopWork()
    {
        if (_state == "Отключен")
            throw new CleanerIsAlreadyStopedException();
        string newState = "Отключен";
        return Create(
            _id,
            _cleanedAmount,
            _lastRun,
            _nextRun,
            _waitDays,
            newState,
            _hours,
            _minutes,
            _seconds
        );
    }

    public ICleaner StartWait()
    {
        if (_state == "Ожидает")
            throw new CleanerIsAlreadyWaitingException();
        string newState = "Ожидает";
        DateTime newLastRun = DateTime.UtcNow;
        DateTime newNextRun = DateTime.UtcNow.AddDays(_waitDays);
        return Create(
            _id,
            _cleanedAmount,
            newLastRun,
            newNextRun,
            _waitDays,
            newState,
            _hours,
            _minutes,
            _seconds
        );
    }

    public ICleaner CleanItem()
    {
        if (_state != "Работает")
            throw new CleanerIsNotBusyException();
        int newCleanedAmount = _cleanedAmount + 1;
        return Create(
            _id,
            newCleanedAmount,
            _lastRun,
            _nextRun,
            _waitDays,
            _state,
            _hours,
            _minutes,
            _seconds
        );
    }

    public ICleaner ChangeWaitDays(int waitDays)
    {
        if (!CanBeConfigured())
            throw new CleanerIsBusyException();
        DateTime newNextRun = DateTime.UtcNow.AddDays(_waitDays);
        return Create(
            _id,
            _cleanedAmount,
            _lastRun,
            newNextRun,
            waitDays,
            _state,
            _hours,
            _minutes,
            _seconds
        );
    }

    public CleanerOutput ProduceOutput()
    {
        return new CleanerOutput(
            _id,
            _cleanedAmount,
            _lastRun,
            _nextRun,
            _waitDays,
            _state,
            _hours,
            _minutes,
            _seconds
        );
    }

    private bool CanBeConfigured()
    {
        if (_state == "Работает" || _state == "Ожидает")
            return false;
        return true;
    }

    private Cleaner(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds
    )
    {
        _id = id;
        _cleanedAmount = cleanedAmount;
        _lastRun = lastRun;
        _nextRun = nextRun;
        _waitDays = waitDays;
        _state = state;
    }

    public static Cleaner Create(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds
    )
    {
        if (id == Guid.Empty)
            throw new CleanerIdIsEmptyException();
        if (cleanedAmount < 0)
            throw new CleanedAmountInvalidException();
        if (lastRun == DateTime.MinValue)
            throw new LastRunInvalidException();
        if (nextRun == DateTime.MinValue)
            throw new NextRunInvalidException();
        if (waitDays <= 0 || waitDays > 7)
            throw new InvalidWaitDaysException(waitDays);
        if (AllowedStates.All(s => s != state))
            throw new StateInvalidException(state);
        return new Cleaner(
            id,
            cleanedAmount,
            lastRun,
            nextRun,
            waitDays,
            state,
            hours,
            minutes,
            seconds
        );
    }
}
