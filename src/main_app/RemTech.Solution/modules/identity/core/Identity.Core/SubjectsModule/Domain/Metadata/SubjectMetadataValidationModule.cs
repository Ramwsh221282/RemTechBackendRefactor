using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Domain.Metadata;

public static class SubjectMetadataValidationModule
{
    public const int MaxLength = 128;
    
    extension(SubjectMetadata metadata)
    {
        internal Result<SubjectMetadata> Validated()
        {
            Guid id = metadata.Id;
            string login = metadata.Login;
            if (Guids.Empty(id)) return Validation("Идентификатор учетной записи пустой.");
            if (Strings.EmptyOrWhiteSpace(login)) return Validation("Логин учетной записи пустой.");
            if (Strings.GreaterThan(login, MaxLength)) return Validation($"Логин учетной записи превышает длину: {MaxLength} символов.");
            return metadata;
        }
    }
}