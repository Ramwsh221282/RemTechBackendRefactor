using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Modules;

public static class IdentitySubjectActivationStatusModule
{
    extension(SubjectActivationStatus)
    {
        public static SubjectActivationStatus Inactive()
        {
            return new SubjectActivationStatus();
        }

        public static Result<SubjectActivationStatus> Create(DateTime activationDate)
        {
            DateTime now = DateTime.UtcNow.AddMinutes(1);
            if (activationDate == DateTime.MinValue || activationDate == DateTime.MaxValue || activationDate > now)
                return Validation("Дата активации учетной записи некорректна.");
            SubjectActivationStatus @new = new(activationDate);
            return Success(@new);
        }
    }
}