namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public class Result
{
	internal Result(Error error)
	{
		Error = error;
		IsSuccess = false;
	}

	internal Result()
	{
		IsSuccess = true;
	}

	public Error Error { get; } = Error.NoError();
	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;

	public static implicit operator Result(Error error)
	{
		return Failure(error);
	}

	public static Result Success() => new();

	public static Result<T> Success<T>(T value) => new(value);

	public static Result<T> Failure<T>(Error error) => new(error);

	public static Result Failure(Error error) => new(error);

	public Result<T> Map<T>(Func<T> onSuccess)
	{
		return IsSuccess ? Success(onSuccess()) : Failure<T>(Error);
	}
}

public class Result<T> : Result
{
	internal Result(T value)
	{
		Value = value;
	}

	internal Result(Error error)
		: base(error)
	{
		Value = default!;
	}

	public T Value =>
		!IsSuccess
			? throw new InvalidOperationException($"Нельзя получить доступ к неуспешному {nameof(Result)}")
			: field;

	public static implicit operator Result<T>(T value)
	{
		return new Result<T>(value);
	}

	public static implicit operator Result<T>(Error error)
	{
		return new Result<T>(error);
	}

	public static implicit operator T(Result<T> result)
	{
		return result.Value;
	}
}
