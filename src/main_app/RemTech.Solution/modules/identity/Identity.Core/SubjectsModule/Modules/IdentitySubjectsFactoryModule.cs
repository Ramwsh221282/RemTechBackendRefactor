using Identity.Core.SubjectsModule.Contexts;
using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Modules;

public static class IdentitySubjectsFactoryModule
{
    extension(IdentitySubject)
    {
        public static Result<IdentitySubject> Create(IdentitySubjectConstructionContext ctx)
        {
            Result<IdentitySubjectMetadata> metadata = ctx.MetaCtx.Construct().Validated();
            Result<IdentitySubjectCredentials> creds = ctx.CredCtx.Construct().Validated();
            Result<IdentitySubjectActivationStatus> activation = ctx.ActivationCtx.Create();
            Result<IdentitySubjectPermissions> permissions = ctx.PermissionsCtx.Create();
            return metadata
                .Continue(creds)
                .Continue(activation)
                .Continue(permissions)
                .Map(() => new IdentitySubject(metadata, creds, activation, permissions));
        }
    }

    extension(IdentitySubjectPermissionsConstructionContext ctx)
    {
        public IdentitySubjectPermissions Empty()
        {
            return new IdentitySubjectPermissions();
        }
        
        public Result<IdentitySubjectPermissions> Create()
        {
            const int maxNameLength = IdentitySubjectPermissions.MAX_NAME_LENGTH;
            if (!ctx.Permissions.All(p => p.Name.Length < maxNameLength))
                Validation($"Название разрешения превышает длину: {maxNameLength}");
            
            Result<UniqueSequence<IdentitySubjectPermission>> unique =
                UniqueSequence<IdentitySubjectPermission>.Create(ctx.Permissions);
            return unique.IsFailure
                ? Validation("Список разрешений не уникален")
                : new IdentitySubjectPermissions(ctx.Permissions);
        }
    }
    
    extension(IdentitySubjectActivationConstructionContext ctx)
    {
        public Result<IdentitySubjectActivationStatus> Create()
        {
            return ctx.ActivationDate.HasValue
                ? IdentitySubjectActivationStatus.Create(ctx.ActivationDate.Value)
                : IdentitySubjectActivationStatus.Inactive();
        }
    }
    
    extension(IdentitySubjectMetadataConstructionContext ctx)
    {
        public IdentitySubjectMetadata Construct()
        {
            Guid id = ctx.Id.HasValue ? ctx.Id.Value : Guid.NewGuid();
            return new IdentitySubjectMetadata(id, ctx.Login);
        }
    }

    extension(IdentitySubjectCredentialsConstructionContext ctx)
    {
        public IdentitySubjectCredentials Construct()
        {
            return new IdentitySubjectCredentials(ctx.Email, ctx.Password);
        }
    }
}