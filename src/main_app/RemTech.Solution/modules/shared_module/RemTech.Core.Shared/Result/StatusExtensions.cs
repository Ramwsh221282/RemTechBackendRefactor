namespace RemTech.Core.Shared.Result;

public static class StatusExtensions
{
    public static Status<TResult> Success<TResult>(this TResult result)
        where TResult : notnull => Status<TResult>.Success(result);
}
