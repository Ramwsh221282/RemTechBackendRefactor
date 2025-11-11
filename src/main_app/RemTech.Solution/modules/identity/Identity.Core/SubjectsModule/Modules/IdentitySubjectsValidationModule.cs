using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Modules;

public static class IdentitySubjectsValidationModule
{
    extension(IdentitySubjectMetadata metadata)
    {
        internal Result<IdentitySubjectMetadata> Validated()
        {
            if (metadata.Id == Guid.Empty) 
                return Validation("Идентификатор был пустым.");
            if (metadata.Login.Length > IdentitySubjectMetadata.MAX_LOGIN_LENGTH)
                return Validation($"Логин превышает длину: {IdentitySubjectMetadata.MAX_LOGIN_LENGTH} символов.");
            return metadata;
        }
    }

    extension(IdentitySubjectCredentials creds)
    {
        internal Result<IdentitySubjectCredentials> Validated()
        {
            EmailString email = EmailStringModule.Create(creds.Email);
            if (!email.IsValid()) return Validation("Некорректный формат почты.");
            return creds;
        }
    }
}