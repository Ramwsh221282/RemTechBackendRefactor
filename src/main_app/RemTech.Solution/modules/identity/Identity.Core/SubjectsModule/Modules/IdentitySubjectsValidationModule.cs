using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions;

namespace Identity.Core.SubjectsModule.Modules;

public static class IdentitySubjectsValidationModule
{
    extension(SubjectMetadata metadata)
    {
        internal Result<SubjectMetadata> Validated()
        {
            if (metadata.Id == Guid.Empty) return Validation("Идентификатор был пустым.");
            if (string.IsNullOrWhiteSpace(metadata.Login)) return Validation("Логин не указан.");
            
            if (metadata.Login.Length > SubjectMetadata.MAX_LOGIN_LENGTH) 
                return Validation($"Логин превышает длину: {SubjectMetadata.MAX_LOGIN_LENGTH} символов.");
            
            return metadata;
        }
    }

    extension(SubjectCredentials creds)
    {
        internal Result<SubjectCredentials> Validated()
        {
            EmailString email = EmailStringModule.Create(creds.Email);
            if (!email.IsValid()) return Validation("Некорректный формат почты.");
            if (string.IsNullOrWhiteSpace(creds.Password)) return Conflict("Пароль не указан.");
            if (creds.Password.Length < 6) return Conflict("Длина пароля менее 6 символов.");
            return creds;
        }
    }
}