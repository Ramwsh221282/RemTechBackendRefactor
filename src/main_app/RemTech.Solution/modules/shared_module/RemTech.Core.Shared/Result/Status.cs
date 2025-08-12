namespace RemTech.Core.Shared.Result;

public class Status
{
    private readonly Error? _error;
    public bool IsSuccess { get; }
    public bool IsFailure { get; }

    public Error Error
    {
        get
        {
            if (IsSuccess)
                throw new ApplicationException("Attempt to access error of success result.");

            if (_error == null)
                throw new ApplicationException("Failed result does not contain error.");

            return _error;
        }
    }

    public Status()
    {
        IsSuccess = true;
        IsFailure = false;
        _error = null;
    }

    public Status(Error error)
    {
        _error = error;
        IsSuccess = false;
        IsFailure = true;
    }

    public Status(ValidationError error)
        : this((Error)error) { }

    public Status(Status status)
    {
        _error = status._error;
        IsSuccess = status.IsSuccess;
        IsFailure = status.IsFailure;
    }

    public static Status Success() => new();

    public static Status Failure(Error error) => new(error);

    public static Status Failures(params Status[] statuses)
    {
        Status? firstBad = statuses.FirstOrDefault(f => f.IsFailure);
        return firstBad == null
            ? throw new ApplicationException("Failure result was not found")
            : new Status(firstBad);
    }

    public static implicit operator Error(Status status) => status.Error;

    public static implicit operator Status(Error error) => Failure(error);
}

public class Status<T> : Status
{
    private readonly T _value;

    public T Value
    {
        get
        {
            if (IsFailure)
                throw new ApplicationException("Attempt to access result of failure.");

            if (_value == null)
                throw new ApplicationException("Success result does not contain value.");

            return _value;
        }
    }

    public Status(T value)
    {
        _value = value;
    }

    public Status(Error error)
        : base(error)
    {
        _value = default!;
    }

    public Status(Status<T> status)
        : base(status.Error)
    {
        _value = status._value;
    }

    public Status(ValidationError error)
        : this((Error)error) { }

    public static Status<T> Failures(params Status<T>[] statuses)
    {
        Status<T>? firstBad = statuses.FirstOrDefault(f => f.IsFailure);
        return firstBad == null
            ? throw new ApplicationException("Failure result was not found")
            : new Status<T>(firstBad);
    }

    public static Status<T> Success(T value) => new(value);

    public static new Status<T> Failure(Error error) => new(error);

    public static implicit operator T(Status<T> status) => status.Value;

    public static implicit operator Error(Status<T> status) => status.Error;

    public static implicit operator Status<T>(Error error) => Failure(error);

    public static implicit operator Status<T>(T value) => Success(value);
}

public sealed class FirstBadStatus
{
    private readonly List<Status> _statuses = [];

    public FirstBadStatus(IEnumerable<Status> statuses)
    {
        _statuses = statuses.ToList();
    }

    public FirstBadStatus(params Status[] statuses)
    {
        _statuses = statuses.ToList();
    }

    public FirstBadStatus Add(Status status)
    {
        if (status.IsSuccess)
            throw new ApplicationException("Статус был удачным.");
        _statuses.Add(status);
        return this;
    }

    public Status Status()
    {
        if (_statuses.All(st => st.IsSuccess))
            throw new ApplicationException(
                "Нельзя найти первый неудачный статус, когда все статусы удачны"
            );

        Status? status = _statuses.FirstOrDefault(s => s.IsFailure);
        if (status == null)
            throw new ApplicationException("Не был найден первый неудачный статус.");

        return status;
    }

    public static implicit operator Status(FirstBadStatus status) => status.Status();

    public static implicit operator Error(FirstBadStatus status) => status.Status();
}
