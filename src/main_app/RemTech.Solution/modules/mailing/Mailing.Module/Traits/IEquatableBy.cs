namespace Mailing.Module.Traits;

public interface IEquatableBy<in T>
{
    bool Equals(T other);
}