namespace Mailing.Module.Traits;

public interface ICreatorOf<out TElement>
{
    TElement Create();
}