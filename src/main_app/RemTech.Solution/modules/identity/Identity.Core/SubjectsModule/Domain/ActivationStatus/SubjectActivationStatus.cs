namespace Identity.Core.SubjectsModule.Domain.ActivationStatus;

public sealed record SubjectActivationStatus
{
    internal Optional<DateTime> ActivationDate { get; init; }

    public Result<SubjectActivationStatus> Activate()
    {
        if (ActivationDate.HasValue) return Conflict("Учетная запись уже активирована.");
        SubjectActivationStatus @new = new(DateTime.UtcNow);
        return Success(@new);
    }

    public bool Activated => ActivationDate.HasValue;
    
    internal SubjectActivationStatus(DateTime activationDate)
    {
        ActivationDate = Some(activationDate);
    }
    
    internal SubjectActivationStatus()
    {
        ActivationDate = None<DateTime>();
    }
}