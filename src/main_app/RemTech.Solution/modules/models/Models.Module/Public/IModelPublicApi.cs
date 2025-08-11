namespace Models.Module.Public;

public interface IModelPublicApi
{
    Task<ModelResponse> Get(string name, CancellationToken cancellationToken = default);
}
