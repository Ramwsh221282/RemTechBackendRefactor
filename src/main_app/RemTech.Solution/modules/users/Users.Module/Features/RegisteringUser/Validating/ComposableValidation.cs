using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Validating;

internal sealed class ComposableValidation : IValidation
{
    private readonly Queue<IValidation> _validations;

    public ComposableValidation()
    {
        _validations = [];
    }

    public ComposableValidation(ComposableValidation validation, IValidation other)
    {
        validation._validations.Enqueue(other);
        _validations = validation._validations;
    }

    public ComposableValidation With(IValidation other)
    {
        return new ComposableValidation(this, other);
    }

    public void Check()
    {
        while (_validations.Count > 0)
            _validations.Dequeue().Check();
    }
}
