namespace Identity.Core.SubjectsModule.Domain.ActivationStatus;

public static class SubjectStatusFactoryModule
{
    extension(SubjectActivationStatus)
    {
        public static Result<SubjectActivationStatus> Create(DateTime date)
        {
            return new SubjectActivationStatus(date).Validated();
        }

        public static Result<SubjectActivationStatus> Create(DateTime? date)
        {
            return date.HasValue ? Create(date.Value) : Create();
        }
        
        public static Result<SubjectActivationStatus> Create()
        {
            return new SubjectActivationStatus().Validated();
        }
    }
}