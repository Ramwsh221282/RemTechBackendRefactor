namespace Mailing.Module.Traits;

public interface IFactoryOf<in TInput, out TOutput> where TInput : FactoryInput
{
    TOutput Create(TInput input);
}