using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Domain.ActivationStatus;

public static class SubjectStatusValidationModule
{
    extension(SubjectActivationStatus status)
    {
        internal Result<SubjectActivationStatus> Validated()
        {
            if (status.ActivationDate.NoValue) return status;
            DateTime date = status.ActivationDate.Value;
            if (date ==  DateTime.MinValue) return status;
            DateTime now = DateTime.UtcNow.AddMinutes(1);
            if (date == DateTime.MinValue || date == DateTime.MaxValue || date > now)
                return Validation("Дата активации учетной записи некорректна.");
            return status;
        }
    }
}