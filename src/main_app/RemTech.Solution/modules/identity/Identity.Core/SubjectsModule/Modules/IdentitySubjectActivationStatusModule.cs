using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Modules;

public static class IdentitySubjectActivationStatusModule
{
    extension(IdentitySubjectActivationStatus)
    {
        public static IdentitySubjectActivationStatus Inactive()
        {
            return new IdentitySubjectActivationStatus();
        }

        public static Result<IdentitySubjectActivationStatus> Create(DateTime activationDate)
        {
            DateTime now = DateTime.UtcNow.AddMinutes(1);
            if (activationDate == DateTime.MinValue || activationDate == DateTime.MaxValue || activationDate > now)
                return Validation("Дата активации учетной записи некорректна.");
            IdentitySubjectActivationStatus @new = new(activationDate);
            return Success(@new);
        }
    }
}