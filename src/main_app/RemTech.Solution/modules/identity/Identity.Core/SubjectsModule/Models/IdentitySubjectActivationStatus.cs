using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Models;

public sealed record IdentitySubjectActivationStatus
{
    private readonly Optional<DateTime> _activationDate;

    public Result<IdentitySubjectActivationStatus> Activate()
    {
        if (_activationDate.HasValue) return Conflict("Учетная запись уже активирована.");
        IdentitySubjectActivationStatus @new = new(DateTime.UtcNow);
        return Success(@new);
    }
    
    internal IdentitySubjectActivationStatus(DateTime activationDate)
    {
        _activationDate = Some(activationDate);
    }
    
    internal IdentitySubjectActivationStatus()
    {
        _activationDate = None<DateTime>();
    }
}