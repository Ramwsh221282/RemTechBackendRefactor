namespace RemTech.Core.Shared.Result;

public class Status
{
    private readonly Error? _error;

    public bool IsSuccess => !IsFailure;

    public bool IsFailure
    {
        get => _error == null || (_error != null && _error == Error.None());
    }

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
        _error = null;
    }

    public Status(Error error)
    {
        _error = error;
    }

    public Status(ValidationError error)
        : this((Error)error)
    {
    }

    public Status(Status status)
    {
        _error = status._error;
    }

    public bool SameErrors(Status other)
    {
        return (_error, other._error) switch
        {
            { Item1: null, Item2: null } => true,
            { Item1: not null, Item2: null } => false,
            { Item1: null, Item2: not null } => false,
            { Item1: Error error1, Item2: Error error2 } => (error1.Code, error2.Code) switch
            {
                _ when error1.Code != error2.Code => false,
                _ when error1.Code == error2.Code => true,
                _ => false,
            },
        };
    }

    public static Status Combined(IEnumerable<Status> statuses)
    {
        if (!statuses.Any())
            throw new ApplicationException("Cannot make a combined status. Enumerable is empty.");

        if (statuses.All(s => s.IsSuccess))
            return Success();

        bool hasFailure = statuses.Any(s => s.IsFailure);
        bool hasSuccess = statuses.Any(s => s.IsSuccess);

        if (hasFailure && hasSuccess)
            throw new ApplicationException("Incompatible statuses. Statuses must be same.");

        var distinctCodes = statuses.Select(s => s.Error.Code).Distinct().ToArray();
        if (distinctCodes.Length > 1)
            throw new ApplicationException(
                "Incompatible statuses. Statuses must be same error code."
            );

        string errorMessage = string.Join(" ,", statuses.Select(s => s.Error.ErrorText));
        return new Status(new Error(errorMessage, distinctCodes[0]));
    }

    public static Status Internal(string message)
    {
        Error error = new Error(message, ErrorCodes.Internal);
        return new Status(error);
    }

    public static Status Validation(string message)
    {
        Error error = new Error(message, ErrorCodes.Validation);
        return new Status(error);
    }

    public static Status Unauthorized() => new Status(Error.Unauthorized());

    public static Status Forbidden() => new Status(Error.Forbidden());

    public static Status Success() => new();

    public static Status Failure(Error error) => new(error);

    public static Status NotFound(string message)
    {
        Error error = new(message, ErrorCodes.NotFound);
        return new Status(error);
    }

    public static implicit operator Error(Status status) => status.Error;

    public static implicit operator Status(Error error) => Failure(error);

    public static Status Conflict(string message)
    {
        Error error = Error.Conflict(message);
        return new Status(error);
    }

    public static Status Forbidden(string message)
    {
        Error error = Error.Forbidden(message);
        return new Status(error);
    }
}

public class Status<T>
{
    private readonly T _value;

    public Error Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

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
        Error = Error.None();
        IsFailure = false;
    }

    public Status(Error error)
    {
        _value = default!;
        Error = error;
        IsFailure = true;
    }

    public Status(Status<T> status)
    {
        _value = status._value;
        Error = status.Error;
        IsFailure = status.IsFailure;
    }

    public Status(ValidationError error)
        : this((Error)error)
    {
    }

    public Status<TResult> Select<TResult>(Func<T, TResult> mapper)
    {
        if (IsFailure)
            return Status<TResult>.Failure(Error);
        return Status<TResult>.Success(mapper(_value));
    }

    public Status<T> Where(Func<Status<T>, bool> mapper)
    {
        if (mapper(this))
            return this;
        return Failure(Error);
    }

    public Status<TResult> SelectMany<TOther, TResult>(
        Func<T, Status<TOther>> selector,
        Func<T, TOther, TResult> resultSelector)
    {
        if (IsFailure)
            return Status<TResult>.Failure(Error);

        var otherStatus = selector(_value);
        if (otherStatus.IsFailure)
            return Status<TResult>.Failure(otherStatus.Error);

        return Status<TResult>.Success(resultSelector(_value, otherStatus._value));
    }

    public static Status<T> None() => new(Error.None());

    public static Status<T> Failures(params Status<T>[] statuses)
    {
        Status<T>? firstBad = statuses.FirstOrDefault(f => f.IsFailure);
        return firstBad == null
            ? throw new ApplicationException("Failure result was not found")
            : new Status<T>(firstBad);
    }

    public static Status<T> Success(T value) => new(value);

    public static Status<T> Failure(Error error) => new(error);

    public static implicit operator T(Status<T> status) => status.Value;

    public static implicit operator Error(Status<T> status) => status.Error;

    public static implicit operator Status(Status<T> status) => new(status.Error);

    public static implicit operator Status<T>(Error error) => Failure(error);

    public static implicit operator Status<T>(T value) => Success(value);
}