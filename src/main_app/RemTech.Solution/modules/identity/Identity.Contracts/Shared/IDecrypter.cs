namespace Identity.Contracts.Shared;

public interface IDecrypter<T> where T : class
{
    Task<T> Decrypt(T value, CancellationToken ct = default);
}