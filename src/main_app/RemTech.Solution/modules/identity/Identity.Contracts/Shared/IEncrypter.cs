namespace Identity.Contracts.Shared;

public interface IEncrypter<T> where T : class
{
    Task<T> Encrypt(T value, CancellationToken ct = default);
}