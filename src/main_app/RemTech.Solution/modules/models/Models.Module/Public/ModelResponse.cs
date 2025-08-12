namespace Models.Module.Public;

public sealed record ModelResponse(Guid Id, string Name, long Rating)
{
    public static async Task<T> MapTo<T>(
        Func<ModelResponse, T> func,
        Func<Task<ModelResponse>> sourceFn
    )
    {
        ModelResponse model = await sourceFn();
        return func(model);
    }
}
