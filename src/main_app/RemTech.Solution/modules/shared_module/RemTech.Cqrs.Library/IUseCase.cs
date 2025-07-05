namespace RemTech.Cqrs.Library;

public interface IUseCase<in TUseCase, TResult>
    where TUseCase : IUseCaseData
{
    Task<TResult> Process(TUseCase data, CancellationToken ct = default);
}
