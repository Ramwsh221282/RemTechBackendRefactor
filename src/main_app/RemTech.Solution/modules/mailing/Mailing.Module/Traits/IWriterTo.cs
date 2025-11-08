namespace Mailing.Module.Traits;

public interface IWriterTo<in TSource>
{
    void Write(TSource target);
}